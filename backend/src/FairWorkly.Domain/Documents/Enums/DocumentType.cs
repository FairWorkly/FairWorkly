namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Types of documents that can be generated
/// Prioritized by legal requirement and business value
/// </summary>
public enum DocumentType
{
  /// <summary>
  /// Fair Work Information Statement
  /// LEGAL REQUIREMENT: Must be given to all new employees before/on first day
  /// Penalty for non-compliance: Up to $13,320 per employee
  /// Priority: Highest (MVP Phase 1)
  /// </summary>
  FairWorkInformationStatement = 1,

  /// <summary>
  /// Separation Certificate
  /// LEGAL REQUIREMENT: Must be provided within 14 days of employment ending
  /// Used by employee to claim Centrelink benefits
  /// Priority: High (MVP Phase 1)
  /// </summary>
  SeparationCertificate = 2,

  /// <summary>
  /// Employment Offer Letter
  /// NOT LEGALLY REQUIRED: But professional businesses provide it
  /// Improves employer branding
  /// Priority: Medium (MVP Phase 1 or Post-MVP)
  /// </summary>
  EmploymentOfferLetter = 3,

  /// <summary>
  /// Casual Conversion Notice
  /// LEGAL REQUIREMENT: Must notify casual employees after 12 months if eligible for conversion
  /// Priority: Medium (Post-MVP - not urgent for new businesses)
  /// </summary>
  CasualConversionNotice = 4,

  /// <summary>
  /// Employment Contract
  /// NOT LEGALLY REQUIRED: Award already defines terms
  /// Some businesses want formal contracts for senior roles
  /// Priority: Low (Post-MVP, optional feature)
  /// </summary>
  EmploymentContract = 5,

  /// <summary>
  /// Position Description
  /// NOT LEGALLY REQUIRED: Professional document
  /// Useful for larger businesses
  /// Priority: Low (Post-MVP, optional feature)
  /// </summary>
  PositionDescription = 6,

  /// <summary>
  /// Other/Custom documents
  /// Future extensibility
  /// </summary>
  Other = 99
}