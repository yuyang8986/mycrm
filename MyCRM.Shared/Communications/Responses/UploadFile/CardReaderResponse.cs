using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Responses.UploadFile
{
    public class CardReaderResponse
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("sid")]
        public string Sid { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("address")]
        public Address[] Address { get; set; }

        [JsonProperty("email")]
        public Email[] Email { get; set; }

        [JsonProperty("formatted_name")]
        public Email[] FormattedName { get; set; }

        [JsonProperty("label")]
        public Label[] Label { get; set; }

        [JsonProperty("name")]
        public Name[] Name { get; set; }

        [JsonProperty("organization")]
        public Organization[] Organization { get; set; }

        [JsonProperty("origin_address")]
        public Email[] OriginAddress { get; set; }

        [JsonProperty("rotation_angle")]
        public long RotationAngle { get; set; }

        [JsonProperty("telephone")]
        public Telephone[] Telephone { get; set; }

        [JsonProperty("title")]
        public Email[] Title { get; set; }

        [JsonProperty("url")]
        public Email[] Url { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("item")]
        public AddressItem Item { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }

    public partial class AddressItem
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postal_code")]
        public long PostalCode { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("type")]
        public string[] Type { get; set; }
    }

    public partial class Email
    {
        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }

    public partial class Label
    {
        [JsonProperty("item")]
        public LabelItem Item { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }

    public partial class LabelItem
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("type")]
        public string[] Type { get; set; }
    }

    public partial class Name
    {
        [JsonProperty("item")]
        public NameItem Item { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }

    public partial class NameItem
    {
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }
    }

    public partial class Organization
    {
        [JsonProperty("item")]
        public OrganizationItem Item { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }

    public partial class OrganizationItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    public partial class Telephone
    {
        [JsonProperty("item")]
        public TelephoneItem Item { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }

    public partial class TelephoneItem
    {
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("type")]
        public string[] Type { get; set; }
    }

   

}
