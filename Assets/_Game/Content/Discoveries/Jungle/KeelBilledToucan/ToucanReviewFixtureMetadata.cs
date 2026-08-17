using PequenoExplorador.Application.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Visuals
{
    [DisallowMultipleComponent]
    public sealed class ToucanReviewFixtureMetadata : MonoBehaviour
    {
        [SerializeField] private string _visualId;
        [SerializeField] private string _futureDiscoveryId;
        [SerializeField] private string _futureInteractionId;
        [SerializeField] private string _author;
        [SerializeField] private string _sourceType;
        [SerializeField] private string _licenseDeclaration;
        [SerializeField] private string _generatorVersion;
        [SerializeField] private string _generatedDate;
        [SerializeField] private EditorialState _editorialState;
        [SerializeField] private bool _isPlaceholder;
        [SerializeField] private string _factualReviewState;
        [SerializeField] private Bounds _candidatePhotoBounds;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private Transform _interactionPoint;
        [SerializeField] private Collider _touchCollider;

        public string VisualId => _visualId;
        public string FutureDiscoveryId => _futureDiscoveryId;
        public string FutureInteractionId => _futureInteractionId;
        public string Author => _author;
        public string SourceType => _sourceType;
        public string LicenseDeclaration => _licenseDeclaration;
        public string GeneratorVersion => _generatorVersion;
        public string GeneratedDate => _generatedDate;
        public EditorialState EditorialState => _editorialState;
        public bool IsPlaceholder => _isPlaceholder;
        public string FactualReviewState => _factualReviewState;
        public Bounds CandidatePhotoBounds => _candidatePhotoBounds;
        public Transform VisualRoot => _visualRoot;
        public Transform InteractionPoint => _interactionPoint;
        public Collider TouchCollider => _touchCollider;
    }
}
