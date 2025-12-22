namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Types of documents to track for compliance
/// Focus on legally required documents
/// </summary>
public enum DocumentType
{
  /// <summary>
  /// Fair Work Information Statement
  /// LEGAL REQUIREMENT: Must be given before/on first day
  /// </summary>
  FairWorkInformationStatement = 1,

  /// <summary>
  /// Separation Certificate
  /// LEGAL REQUIREMENT: Within 14 days of termination
  /// </summary>
  SeparationCertificate = 2,

  /// <summary>
  /// Casual Conversion Notice
  /// LEGAL REQUIREMENT: After 12 months for eligible casuals
  /// </summary>
  CasualConversionNotice = 3,

  /// <summary>
  /// Employment Offer Letter
  /// NOT REQUIRED: Optional professional document
  /// </summary>
  EmploymentOfferLetter = 4
}