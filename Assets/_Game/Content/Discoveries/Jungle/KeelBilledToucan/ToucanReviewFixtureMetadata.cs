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
        [SerializeField] private string _visualReviewState;
        [SerializeField] private string _visualApprovedBy;
        [SerializeField] private string _visualApprovalDate;
        [SerializeField] private string _visualApprovalReference;
        [SerializeField] private string _factualReviewState;
        [SerializeField] private string _factualReviewedBy;
        [SerializeField] private string _factualReviewerCompetence;
        [SerializeField] private string _factualReviewDate;
        [SerializeField] private string _factualApprovalReference;
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
        public string VisualReviewState => _visualReviewState;
        public string VisualApprovedBy => _visualApprovedBy;
        public string VisualApprovalDate => _visualApprovalDate;
        public string VisualApprovalReference => _visualApprovalReference;
        public string FactualReviewState => _factualReviewState;
        public string FactualReviewedBy => _factualReviewedBy;
        public string FactualReviewerCompetence => _factualReviewerCompetence;
        public string FactualReviewDate => _factualReviewDate;
        public string FactualApprovalReference => _factualApprovalReference;
        public Bounds CandidatePhotoBounds => _candidatePhotoBounds;
        public Transform VisualRoot => _visualRoot;
        public Transform InteractionPoint => _interactionPoint;
        public Collider TouchCollider => _touchCollider;
    }
}
