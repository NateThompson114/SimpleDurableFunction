using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SimpleDurableFunction.Models;

[DataContract(Namespace = "")]
public class PaymentMessage
{
    [DataMember]
    public PaymentMessageHeader Header { get; set; } = (PaymentMessageHeader)null;

    [DataMember]
    [Required]
    public PaymentBatch PaymentBatch { get; set; } = (PaymentBatch)null;
}

[DataContract(Namespace = "")]
public class PaymentMessageHeader
{
    [DataMember]
    public int BatchId { get; set; }

    [DataMember]
    public string ClientBatchID { get; set; } = (string)null;

    [DataMember]
    public Guid MessageId { get; set; }

    [DataMember]
    public int OrganizationId { get; set; }

    [DataMember]
    public int PaymentCount { get; set; }

    [DataMember]
    public int UserId { get; set; }
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
[XmlRoot(ElementName = "PaymentBatch", Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class PaymentBatch
{
    [DataMember(IsRequired = true)]
    [Required]
    public Identifier ExternalId { get; set; } = (Identifier)null;

    [DataMember]
    public Identifier Id { get; set; } = (Identifier)null;

    [DataMember(IsRequired = true)]
    public Collection<Payment> Payments { get; set; } = (Collection<Payment>)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class Identifier
{
    [DataMember]
    [StringLength(50)]
    public string Scheme { get; set; } = (string)null;

    [DataMember]
    public PartyRole SchemeOwner { get; set; }

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Value { get; set; } = (string)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public enum PartyRole
{
    [EnumMember] Buyer,
    [EnumMember] Provider,
    [EnumMember] Aggregator,
    [EnumMember] AvidXchange,
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class IdentifierNoMin
{
    [DataMember(IsRequired = true)]
    [StringLength(100)]
    public string Value { get; set; } = (string)null;

    [DataMember]
    public PartyRole SchemeOwner { get; set; }

    [DataMember]
    [StringLength(50)]
    public string Scheme { get; set; } = (string)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class BankAccountDetail
{
    [DataMember]
    [StringLength(50)]
    public string AccountNum { get; set; } = (string)null;

    [DataMember]
    public IdentifierNoMin ClientId { get; set; } = (IdentifierNoMin)null;

    [DataMember]
    [StringLength(9)]
    public string RoutingNum { get; set; } = (string)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class Check
{
    [DataMember(IsRequired = true)]
    [Required]
    public Decimal Amount { get; set; }

    [DataMember(IsRequired = true)]
    [Required]
    public DateTime Date { get; set; }

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Length must have a minimum length of one value and a maximum length of 10 values.")]
    public string Number { get; set; } = (string)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public enum DeliveryMethod
{
    [EnumMember] PriorityOvernight,
    [EnumMember] StandardOvernight,
    [EnumMember] TwoDay,
    [EnumMember] ThreeDay,
    [EnumMember] Mail,
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class Address
{
    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string City { get; set; } = (string)null;

    [DataMember]
    [StringLength(3, MinimumLength = 2)]
    public string CountryCode { get; set; } = (string)null;

    [DataMember]
    public IdentifierNoMin ExternalId { get; set; } = (IdentifierNoMin)null;

    [DataMember]
    public Identifier Id { get; set; } = (Identifier)null;

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string Line1 { get; set; } = (string)null;

    [DataMember]
    [StringLength(255)]
    public string Line2 { get; set; } = (string)null;

    [DataMember]
    [StringLength(255)]
    public string Line3 { get; set; } = (string)null;

    [DataMember]
    [StringLength(255)]
    public string Line4 { get; set; } = (string)null;

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string PostalCode { get; set; } = (string)null;

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string State { get; set; } = (string)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class Payee
{
    [DataMember(IsRequired = true)]
    public Address Address { get; set; } = (Address)null;

    [DataMember]
    [StringLength(255, MinimumLength = 1)]
    public string DoingBusinessAs { get; set; } = (string)null;

    [DataMember]
    [StringLength(255)]
    public string EmailAddress { get; set; } = (string)null;

    [DataMember]
    public IdentifierNoMin ExternalId { get; set; } = (IdentifierNoMin)null;

    [DataMember]
    public Identifier Id { get; set; } = (Identifier)null;

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = (string)null;

    [DataMember]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = (string)null;

    [DataMember(IsRequired = true)]
    [Required]
    public Address RemitToAddress { get; set; } = (Address)null;

    [DataMember]
    public IdentifierNoMin TaxId { get; set; } = (IdentifierNoMin)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class Payer
{
    [DataMember(IsRequired = true)]
    [Required]
    public Address Address { get; set; } = (Address)null;

    [DataMember(IsRequired = false)]
    [StringLength(255)]
    public string AlternateName { get; set; } = (string)null;

    [DataMember]
    public IdentifierNoMin ExternalId { get; set; } = (IdentifierNoMin)null;

    [DataMember]
    public Identifier Id { get; set; } = (Identifier)null;

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = (string)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class RemittanceDetail
{
    [DataMember]
    public string AvidInvoiceURL { get; set; } = (string)null;

    [DataMember]
    [StringLength(100)]
    public string CustomerAccountNumber { get; set; } = (string)null;

    [DataMember(IsRequired = true)]
    [Required]
    public DateTime? Date { get; set; }

    [DataMember]
    public string Description { get; set; } = (string)null;

    [DataMember(IsRequired = true)]
    [Required]
    public Decimal? Discount { get; set; }

    [DataMember]
    public DateTime? DueDate { get; set; }

    [DataMember]
    public IdentifierNoMin ExternalId { get; set; } = (IdentifierNoMin)null;

    [DataMember(IsRequired = true)]
    [Required]
    public Decimal? Gross { get; set; }

    [DataMember(IsRequired = true)]
    [Required]
    public Decimal? Net { get; set; }

    [DataMember(IsRequired = true)]
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Number { get; set; } = (string)null;
}

[DataContract(Namespace = "http://example.com/service/messages/paymentbatch/v2")]
public class Payment
{
    [DataMember(IsRequired = false)]
    public int? AccountingSystemId { get; set; }

    [DataMember]
    public BankAccountDetail BankAccountDetail { get; set; } = (BankAccountDetail)null;

    [DataMember(IsRequired = true)]
    public Check Check { get; set; } = (Check)null;

    [DataMember(IsRequired = true)]
    public DeliveryMethod? DeliveryMethod { get; set; }

    [DataMember]
    public bool DuplicateCheckInBatch { get; set; }

    [DataMember]
    public bool DuplicateExternalIdInBatch { get; set; }

    [DataMember(IsRequired = true)]
    [Required]
    public DateTime? ExpenseDate { get; set; }

    [DataMember]
    public IdentifierNoMin ExternalId { get; set; } = (IdentifierNoMin)null;

    [DataMember]
    public Identifier Id { get; set; } = (Identifier)null;

    [DataMember(IsRequired = false)]
    public int? InstanceId { get; set; } = new int?();

    [DataMember(IsRequired = true)]
    public Payee Payee { get; set; } = (Payee)null;

    [DataMember(IsRequired = true)]
    public Payer Payer { get; set; } = (Payer)null;

    [DataMember]
    public Identifier ProviderId { get; set; } = (Identifier)null;

    [DataMember]
    [StringLength(255)]
    public string Reference { get; set; } = (string)null;

    [DataMember(IsRequired = true)]
    public List<RemittanceDetail> Remittances { get; set; } = (List<RemittanceDetail>)null;

    [DataMember(IsRequired = true)]
    public bool? ReturnToPayer { get; set; }

    [DataMember(IsRequired = true)]
    public bool? SendPaperCheck { get; set; }
}