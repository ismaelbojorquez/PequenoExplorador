using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Album;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Domain.Content;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Album
{
    [DisallowMultipleComponent]
    public sealed class AlbumView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_ALBUM";
        public const int EntriesPerPage = 8;
        public const int MaximumCachedPhotos = 8;

        [SerializeField] private Button _openButton;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _title;
        [SerializeField] private Text _progress;
        [SerializeField] private AlbumCategoryCell[] _categoryCells = Array.Empty<AlbumCategoryCell>();
        [SerializeField] private AlbumEntryCell[] _entryCells = Array.Empty<AlbumEntryCell>();
        [SerializeField] private Button _previousPageButton;
        [SerializeField] private Button _nextPageButton;
        [SerializeField] private Text _pageText;
        [SerializeField] private GameObject _loadingState;
        [SerializeField] private GameObject _emptyState;
        [SerializeField] private GameObject _errorState;
        [SerializeField] private Text _stateText;
        [SerializeField] private GameObject _detailPanel;
        [SerializeField] private Button _detailBackButton;
        [SerializeField] private Image _detailImage;
        [SerializeField] private Text _detailName;
        [SerializeField] private Text[] _factLabels = Array.Empty<Text>();
        [SerializeField] private Text[] _factValues = Array.Empty<Text>();
        [SerializeField] private Button _replayButton;
        [SerializeField] private Text _replayLabel;
        [SerializeField] private Text _photoState;

        private readonly Dictionary<string, CachedPhoto> _photoCache = new Dictionary<string, CachedPhoto>(StringComparer.Ordinal);
        private readonly Queue<string> _photoCacheOrder = new Queue<string>();
        private AlbumQueryService _query;
        private IPhotoStore _photos;
        private ILocalizationService _localization;
        private IAudioService _audio;
        private ISceneFlowService _sceneFlow;
        private WorldId _worldId;
        private CategoryId? _selectedCategory;
        private AlbumSnapshot _snapshot;
        private AlbumEntryViewModel _selectedEntry;
        private CancellationTokenSource _loadCancellation;
        private int _page;
        private int _generation;

        public bool IsVisible => _panel != null && _panel.activeSelf;
        public bool IsDetailVisible => _detailPanel != null && _detailPanel.activeSelf;
        public bool IsOpenAvailable => _openButton != null && _openButton.gameObject.activeSelf;
        public int VisibleEntryCount => _entryCells.Count(item => item != null && item.gameObject.activeSelf);
        public int CurrentPage => _page;
        public AlbumSnapshot Snapshot => _snapshot;
        public string StateText => _stateText == null ? string.Empty : _stateText.text;
        public Button OpenButton => _openButton;
        public AlbumEntryCell FirstVisibleEntryCell => _entryCells.FirstOrDefault(item => item != null && item.gameObject.activeSelf);
        public string DetailNameText => _detailName == null ? string.Empty : _detailName.text;
        public Sprite DetailPhotoSprite => _detailImage == null ? null : _detailImage.sprite;
        public bool ReplayInteractable => _replayButton != null && _replayButton.interactable;

        private void Awake()
        {
            _openButton?.onClick.AddListener(Open);
            _backButton?.onClick.AddListener(Close);
            _detailBackButton?.onClick.AddListener(CloseDetail);
            _previousPageButton?.onClick.AddListener(PreviousPage);
            _nextPageButton?.onClick.AddListener(NextPage);
            _replayButton?.onClick.AddListener(Replay);
        }

        public void Bind(
            AlbumQueryService query,
            IPhotoStore photos,
            ILocalizationService localization,
            IAudioService audio,
            ISceneFlowService sceneFlow,
            WorldId worldId)
        {
            Unbind();
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _photos = photos ?? throw new ArgumentNullException(nameof(photos));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
            _sceneFlow = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
            if (!worldId.IsValid) throw new ArgumentException("Album world ID is invalid.", nameof(worldId));
            _worldId = worldId;
            _localization.LocaleChanged += HandleLocaleChanged;
            _sceneFlow.Changed += HandleSceneChanged;
            HideAll();
            HandleSceneChanged(_sceneFlow.Snapshot);
        }

        public void Open()
        {
            if (_sceneFlow == null || _sceneFlow.Snapshot.Current != SceneFlowState.Camp || _sceneFlow.Snapshot.IsTransitioning) return;
            if (_panel != null) _panel.SetActive(true);
            if (_detailPanel != null) _detailPanel.SetActive(false);
            _selectedEntry = null;
            _page = 0;
            Refresh();
        }

        public void Close()
        {
            CancelLoads();
            if (_panel != null) _panel.SetActive(false);
            if (_detailPanel != null) _detailPanel.SetActive(false);
            _selectedEntry = null;
        }

        public bool TryHandleBack()
        {
            if (IsDetailVisible) { CloseDetail(); return true; }
            if (IsVisible) { Close(); return true; }
            return false;
        }

        public void Refresh()
        {
            if (_query == null || !IsVisible) return;
            CancelLoads();
            ShowState(_loadingState, LocalizationKeys.AlbumLoading);
            try
            {
                _snapshot = _query.Query(_worldId, _selectedCategory);
                RenderSnapshot();
            }
            catch (Exception)
            {
                _snapshot = null;
                ShowState(_errorState, LocalizationKeys.AlbumError);
            }
        }

        private void RenderSnapshot()
        {
            HideStates();
            if (_title != null) _title.text = Resolve(LocalizationKeys.AlbumTitle);
            if (_progress != null)
                _progress.text = Resolve(LocalizationKeys.AlbumWorldProgress, _snapshot.Discovered, _snapshot.Total);
            BindCategories();
            if (_snapshot.Entries.Count == 0)
            {
                ClearEntries();
                ShowState(_emptyState, LocalizationKeys.AlbumEmpty);
                return;
            }
            RenderPage();
        }

        private void BindCategories()
        {
            for (int index = 0; index < _categoryCells.Length; index++)
            {
                AlbumCategoryCell cell = _categoryCells[index];
                if (cell == null) continue;
                if (index >= _snapshot.Categories.Count) { cell.Clear(); continue; }
                AlbumCategoryViewModel category = _snapshot.Categories[index];
                cell.Bind(category, _localization, SelectCategory);
                cell.SetSelected(_selectedCategory.HasValue && _selectedCategory.Value.Equals(category.Id));
            }
        }

        private void RenderPage()
        {
            CancelLoads();
            int pageCount = Math.Max(1, (_snapshot.Entries.Count + EntriesPerPage - 1) / EntriesPerPage);
            _page = Math.Max(0, Math.Min(_page, pageCount - 1));
            int offset = _page * EntriesPerPage;
            for (int index = 0; index < _entryCells.Length; index++)
            {
                AlbumEntryCell cell = _entryCells[index];
                if (cell == null) continue;
                int entryIndex = offset + index;
                if (index >= EntriesPerPage || entryIndex >= _snapshot.Entries.Count) { cell.Clear(); continue; }
                cell.Bind(_snapshot.Entries[entryIndex], _localization, SelectEntry);
            }
            if (_pageText != null) _pageText.text = Resolve(LocalizationKeys.AlbumPage, _page + 1, pageCount);
            if (_previousPageButton != null) _previousPageButton.interactable = _page > 0;
            if (_nextPageButton != null) _nextPageButton.interactable = _page + 1 < pageCount;
            BeginGridPhotoLoads();
        }

        private void SelectCategory(AlbumCategoryViewModel category)
        {
            _selectedCategory = _selectedCategory.HasValue && _selectedCategory.Value.Equals(category.Id)
                ? (CategoryId?)null
                : category.Id;
            _page = 0;
            Refresh();
        }

        private void SelectEntry(AlbumEntryViewModel entry)
        {
            if (!entry.IsDiscovered)
            {
                ShowState(_emptyState, LocalizationKeys.AlbumLockedHint);
                return;
            }
            HideStates();
            _selectedEntry = entry;
            if (_detailPanel != null) _detailPanel.SetActive(true);
            RenderDetailText();
            BeginDetailPhotoLoad(entry);
        }

        private void RenderDetailText()
        {
            if (_selectedEntry == null || _localization == null) return;
            if (_detailName != null) _detailName.text = Resolve(_selectedEntry.DisplayName);
            AlbumFactField[] ordered = { AlbumFactField.Habitat, AlbumFactField.Diet, AlbumFactField.Size, AlbumFactField.Curiosity, AlbumFactField.Sound };
            for (int index = 0; index < ordered.Length; index++)
            {
                if (index < _factLabels.Length && _factLabels[index] != null)
                    _factLabels[index].text = Resolve(FactLabelKey(ordered[index]));
                AlbumFactViewModel fact = _selectedEntry.Facts.FirstOrDefault(item => item.Field == ordered[index]);
                if (index < _factValues.Length && _factValues[index] != null)
                    _factValues[index].text = fact.HasApprovedValue
                        ? Resolve(fact.Value)
                        : Resolve(LocalizationKeys.AlbumFactPending);
            }
            if (_replayButton != null) _replayButton.interactable = _selectedEntry.HasPlayableAudio;
            if (_replayLabel != null)
                _replayLabel.text = Resolve(_selectedEntry.HasPlayableAudio
                    ? LocalizationKeys.AlbumReplay
                    : LocalizationKeys.AlbumAudioPending);
            if (_photoState != null)
                _photoState.text = Resolve(_selectedEntry.HasPhotoReference
                    ? LocalizationKeys.AlbumPhotoLoading
                    : LocalizationKeys.AlbumCanonicalFallback);
            ApplyDetailPhoto(null);
        }

        private void CloseDetail()
        {
            CancelLoads();
            if (_detailPanel != null) _detailPanel.SetActive(false);
            _selectedEntry = null;
            RenderPage();
        }

        private void PreviousPage() { if (_page <= 0) return; _page--; RenderPage(); }
        private void NextPage()
        {
            if (_snapshot == null || (_page + 1) * EntriesPerPage >= _snapshot.Entries.Count) return;
            _page++; RenderPage();
        }

        private void Replay()
        {
            if (_selectedEntry?.HasPlayableAudio == true) _audio.Play(_selectedEntry.AudioCueId);
        }

        private void BeginGridPhotoLoads()
        {
            CancellationToken token = CreateLoadToken();
            int generation = _generation;
            _ = LoadGridPhotosAsync(generation, token);
        }

        private async Task LoadGridPhotosAsync(int generation, CancellationToken token)
        {
            try
            {
                foreach (AlbumEntryCell cell in _entryCells.Where(item => item != null && item.gameObject.activeSelf))
                {
                    AlbumEntryViewModel entry = cell.Model;
                    if (entry == null || !entry.IsDiscovered || !entry.HasPhotoReference) continue;
                    Sprite sprite = await LoadPhotoSpriteAsync(entry.PhotoFileReference, token);
                    if (token.IsCancellationRequested || generation != _generation || cell.Model?.Id != entry.Id) return;
                    cell.ApplyPhoto(sprite);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void BeginDetailPhotoLoad(AlbumEntryViewModel entry)
        {
            if (!entry.HasPhotoReference) return;
            CancellationToken token = CreateLoadToken();
            int generation = _generation;
            _ = LoadDetailPhotoAsync(entry, generation, token);
        }

        private async Task LoadDetailPhotoAsync(AlbumEntryViewModel entry, int generation, CancellationToken token)
        {
            try
            {
                Sprite sprite = await LoadPhotoSpriteAsync(entry.PhotoFileReference, token);
                if (token.IsCancellationRequested || generation != _generation || _selectedEntry?.Id != entry.Id) return;
                ApplyDetailPhoto(sprite);
                if (_photoState != null)
                    _photoState.text = Resolve(sprite == null
                        ? LocalizationKeys.AlbumCanonicalFallback
                        : LocalizationKeys.AlbumBestPhoto);
            }
            catch (OperationCanceledException) { }
        }

        private async Task<Sprite> LoadPhotoSpriteAsync(string reference, CancellationToken token)
        {
            if (_photoCache.TryGetValue(reference, out CachedPhoto cached)) return cached.Sprite;
            PhotoLoadResult loaded = await _photos.LoadAsync(reference, token);
            token.ThrowIfCancellationRequested();
            if (!loaded.IsLoaded) return null;
            var texture = new Texture2D(2, 2, TextureFormat.RGB24, false, false)
            {
                name = "AlbumPhoto_" + reference,
                hideFlags = HideFlags.DontSave
            };
            if (!texture.LoadImage(loaded.PngBytes, markNonReadable: true))
            {
                Destroy(texture);
                return null;
            }
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = "AlbumPhotoSprite_" + reference;
            AddCachedPhoto(reference, new CachedPhoto(texture, sprite));
            return sprite;
        }

        private void AddCachedPhoto(string reference, CachedPhoto photo)
        {
            _photoCache[reference] = photo;
            _photoCacheOrder.Enqueue(reference);
            while (_photoCacheOrder.Count > MaximumCachedPhotos)
            {
                string oldest = _photoCacheOrder.Dequeue();
                if (_photoCache.Remove(oldest, out CachedPhoto removed)) removed.Dispose();
            }
        }

        private void ApplyDetailPhoto(Sprite sprite)
        {
            if (_detailImage == null) return;
            _detailImage.sprite = sprite;
            _detailImage.preserveAspect = true;
            _detailImage.color = sprite == null ? new Color(0.22f, 0.55f, 0.43f, 1f) : Color.white;
        }

        private CancellationToken CreateLoadToken()
        {
            CancelLoads();
            _loadCancellation = new CancellationTokenSource();
            _generation++;
            return _loadCancellation.Token;
        }

        private void CancelLoads()
        {
            _loadCancellation?.Cancel();
            _loadCancellation?.Dispose();
            _loadCancellation = null;
            _generation++;
        }

        private void HandleLocaleChanged(string locale)
        {
            if (!IsVisible) return;
            SetButtonLabel(_openButton, LocalizationKeys.AlbumOpen);
            SetButtonLabel(_backButton, LocalizationKeys.AlbumBack);
            SetButtonLabel(_detailBackButton, LocalizationKeys.AlbumBack);
            SetButtonLabel(_previousPageButton, LocalizationKeys.AlbumPrevious);
            SetButtonLabel(_nextPageButton, LocalizationKeys.AlbumNext);
            if (IsDetailVisible)
            {
                RenderDetailText();
                if (_selectedEntry != null) BeginDetailPhotoLoad(_selectedEntry);
            }
            else RenderSnapshot();
        }

        private void HandleSceneChanged(SceneFlowSnapshot snapshot)
        {
            bool available = snapshot != null && !snapshot.IsTransitioning && snapshot.Current == SceneFlowState.Camp;
            if (_openButton != null) _openButton.gameObject.SetActive(available);
            if (!available) Close();
            SetButtonLabel(_openButton, LocalizationKeys.AlbumOpen);
            SetButtonLabel(_backButton, LocalizationKeys.AlbumBack);
            SetButtonLabel(_detailBackButton, LocalizationKeys.AlbumBack);
            SetButtonLabel(_previousPageButton, LocalizationKeys.AlbumPrevious);
            SetButtonLabel(_nextPageButton, LocalizationKeys.AlbumNext);
        }

        private void ShowState(GameObject state, LocalizedKey text)
        {
            HideStates();
            if (state != null) state.SetActive(true);
            if (_stateText != null)
            {
                _stateText.gameObject.SetActive(true);
                _stateText.text = Resolve(text);
            }
        }

        private void HideStates()
        {
            if (_loadingState != null) _loadingState.SetActive(false);
            if (_emptyState != null) _emptyState.SetActive(false);
            if (_errorState != null) _errorState.SetActive(false);
            if (_stateText != null) _stateText.gameObject.SetActive(false);
        }

        private void ClearEntries()
        {
            foreach (AlbumEntryCell cell in _entryCells)
            {
                if (cell != null) cell.Clear();
            }
        }

        private void HideAll()
        {
            if (_panel != null) _panel.SetActive(false);
            if (_detailPanel != null) _detailPanel.SetActive(false);
            if (_openButton != null) _openButton.gameObject.SetActive(false);
            HideStates();
        }

        private string Resolve(LocalizedKey key, params object[] arguments)
        {
            try { return _localization?.Resolve(key, arguments) ?? string.Empty; }
            catch (InvalidOperationException) { return string.Empty; }
        }

        private void SetButtonLabel(Button button, LocalizedKey key)
        {
            Text text = button == null ? null : button.GetComponentInChildren<Text>(true);
            if (text != null) text.text = Resolve(key);
        }

        private static LocalizedKey FactLabelKey(AlbumFactField field)
        {
            switch (field)
            {
                case AlbumFactField.Habitat: return LocalizationKeys.AlbumHabitat;
                case AlbumFactField.Diet: return LocalizationKeys.AlbumDiet;
                case AlbumFactField.Size: return LocalizationKeys.AlbumSize;
                case AlbumFactField.Curiosity: return LocalizationKeys.AlbumCuriosity;
                default: return LocalizationKeys.AlbumSound;
            }
        }

        public void Unbind()
        {
            Close();
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_sceneFlow != null) _sceneFlow.Changed -= HandleSceneChanged;
            _query = null;
            _photos = null;
            _localization = null;
            _audio = null;
            _sceneFlow = null;
            _snapshot = null;
            _selectedEntry = null;
            _selectedCategory = null;
            ClearEntries();
            foreach (AlbumCategoryCell cell in _categoryCells)
            {
                if (cell != null) cell.Clear();
            }
            while (_photoCacheOrder.Count > 0) _photoCacheOrder.Dequeue();
            foreach (CachedPhoto photo in _photoCache.Values) photo.Dispose();
            _photoCache.Clear();
        }

#if UNITY_EDITOR
        public void ConfigureForEditorAndTests(
            Button openButton, GameObject panel, Button backButton, Text title, Text progress,
            AlbumCategoryCell[] categoryCells, AlbumEntryCell[] entryCells,
            Button previousPage, Button nextPage, Text pageText,
            GameObject loading, GameObject empty, GameObject error, Text stateText,
            GameObject detailPanel, Button detailBack, Image detailImage, Text detailName,
            Text[] factLabels, Text[] factValues, Button replay, Text replayLabel, Text photoState)
        {
            _openButton = openButton; _panel = panel; _backButton = backButton; _title = title; _progress = progress;
            _categoryCells = categoryCells ?? Array.Empty<AlbumCategoryCell>();
            _entryCells = entryCells ?? Array.Empty<AlbumEntryCell>();
            _previousPageButton = previousPage; _nextPageButton = nextPage; _pageText = pageText;
            _loadingState = loading; _emptyState = empty; _errorState = error; _stateText = stateText;
            _detailPanel = detailPanel; _detailBackButton = detailBack; _detailImage = detailImage; _detailName = detailName;
            _factLabels = factLabels ?? Array.Empty<Text>(); _factValues = factValues ?? Array.Empty<Text>();
            _replayButton = replay; _replayLabel = replayLabel; _photoState = photoState;
        }
#endif

        private void OnDestroy()
        {
            _openButton?.onClick.RemoveListener(Open);
            _backButton?.onClick.RemoveListener(Close);
            _detailBackButton?.onClick.RemoveListener(CloseDetail);
            _previousPageButton?.onClick.RemoveListener(PreviousPage);
            _nextPageButton?.onClick.RemoveListener(NextPage);
            _replayButton?.onClick.RemoveListener(Replay);
            Unbind();
        }

        private sealed class CachedPhoto
        {
            public CachedPhoto(Texture2D texture, Sprite sprite) { Texture = texture; Sprite = sprite; }
            public Texture2D Texture { get; }
            public Sprite Sprite { get; }
            public void Dispose() { if (Sprite != null) UnityEngine.Object.Destroy(Sprite); if (Texture != null) UnityEngine.Object.Destroy(Texture); }
        }
    }
}
