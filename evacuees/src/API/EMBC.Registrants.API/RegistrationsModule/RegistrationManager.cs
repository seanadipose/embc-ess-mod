﻿// -------------------------------------------------------------------------
//  Copyright © 2020 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  https://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EMBC.ResourceAccess.Dynamics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Dynamics.CRM;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Microsoft.OData.Edm;

namespace EMBC.Registrants.API.RegistrationsModule
{
    public interface IRegistrationManager
    {
        Task<string> CreateRegistrationAnonymous(AnonymousRegistration registration);
        Task<OkResult> CreateProfile(Registration profileRegistration);
    }

    public class RegistrationManager : IRegistrationManager
    {
        private readonly DynamicsClientContext dynamicsClient;
#pragma warning disable 0649
        private readonly ILogger logger;
#pragma warning restore 0649

        public RegistrationManager(DynamicsClientContext dynamicsClient)
        {
            this.dynamicsClient = dynamicsClient;
        }

        public async Task<OkResult> CreateProfile(Registration profileRegistration)
        {
            var now = DateTimeOffset.UtcNow;

            // registrant
            var registrant = new contact
            {
                contactid = Guid.NewGuid(),
                era_registranttype = 174360000,
                era_authenticated = true,
                era_verified = false,
                era_registrationdate = now,
                firstname = profileRegistration.PersonalDetails.FirstName,
                lastname = profileRegistration.PersonalDetails.LastName,
                era_preferredname = profileRegistration.PersonalDetails.PreferredName,
                era_initial = profileRegistration.PersonalDetails.Initials,
                gendercode = LookupGender(profileRegistration.PersonalDetails.Gender),
                birthdate = FromDateTime(DateTime.Parse(profileRegistration.PersonalDetails.DateOfBirth)),
                era_collectionandauthorization = profileRegistration.InformationCollectionConsent,
                era_sharingrestriction = profileRegistration.RestrictedAccess,

                address1_line1 = profileRegistration.PrimaryAddress.AddressLine1,
                address1_line2 = profileRegistration.PrimaryAddress.AddressLine2,
                address1_city = profileRegistration.PrimaryAddress.Jurisdiction.JurisdictionName,
                address1_country = profileRegistration.PrimaryAddress.Country.CountryCode,
                era_City = Lookup(profileRegistration.PrimaryAddress.Jurisdiction),
                era_ProvinceState = Lookup(profileRegistration.PrimaryAddress.StateProvince),
                era_Country = Lookup(profileRegistration.PrimaryAddress.Country),
                address1_postalcode = profileRegistration.PrimaryAddress.PostalCode,

                address2_line1 = profileRegistration.MailingAddress.AddressLine1,
                address2_line2 = profileRegistration.MailingAddress.AddressLine2,
                address2_city = profileRegistration.MailingAddress.Jurisdiction.JurisdictionName,
                address2_country = profileRegistration.MailingAddress.Country.CountryName,
                era_MailingCity = Lookup(profileRegistration.MailingAddress.Jurisdiction),
                era_MailingProvinceState = Lookup(profileRegistration.MailingAddress.StateProvince),
                era_MailingCountry = Lookup(profileRegistration.MailingAddress.Country),
                address2_postalcode = profileRegistration.MailingAddress.PostalCode,

                emailaddress1 = profileRegistration.ContactDetails.Email,
                address1_telephone1 = profileRegistration.ContactDetails.Phone,

                era_phonenumberrefusal = string.IsNullOrEmpty(profileRegistration.ContactDetails.Phone),
                era_emailrefusal = string.IsNullOrEmpty(profileRegistration.ContactDetails.Email),
                era_secrettext = profileRegistration.SecretPhrase
            };

            // save registrant to dynamics
            dynamicsClient.AddTocontacts(registrant);

            var results = await dynamicsClient.SaveChangesAsync(SaveChangesOptions.ContinueOnError);

            return new OkResult();
        }

        public async Task<string> CreateRegistrationAnonymous(AnonymousRegistration registration)
        {
            var now = DateTimeOffset.Now;
#pragma warning disable CA5394 // Do not use insecure randomness
            var essFileNumber = new Random().Next(999999999); //temporary ESS file number random generator
#pragma warning restore CA5394 // Do not use insecure randomness

            // evacuation file
            var file = new era_evacuationfile
            {
                era_evacuationfileid = Guid.NewGuid(),
                era_essfilenumber = essFileNumber,
                era_evacuationfiledate = now,
                era_addressline1 = registration.PreliminaryNeedsAssessment.EvacuatedFromAddress.AddressLine1,
                era_addressline2 = registration.PreliminaryNeedsAssessment.EvacuatedFromAddress.AddressLine2,
                era_city = registration.PreliminaryNeedsAssessment.EvacuatedFromAddress.AddressLine1,
                era_Jurisdiction = Lookup(registration.PreliminaryNeedsAssessment.EvacuatedFromAddress.Jurisdiction),
                era_province = registration.PreliminaryNeedsAssessment.EvacuatedFromAddress.StateProvince.StateProvinceCode,
                era_country = registration.PreliminaryNeedsAssessment.EvacuatedFromAddress.Country.CountryCode,
                era_collectionandauthorization = registration.RegistrationDetails.InformationCollectionConsent,
                era_sharingrestriction = registration.RegistrationDetails.RestrictedAccess,
                era_phonenumberrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Phone),
                era_emailrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Email),
                era_secrettext = registration.RegistrationDetails.SecretPhrase,
            };

            // registrant
            var registrant = new contact
            {
                contactid = Guid.NewGuid(),
                era_registranttype = 174360000,
                era_authenticated = false,
                era_verified = false,
                era_registrationdate = now,
                firstname = registration.RegistrationDetails.PersonalDetails.FirstName,
                lastname = registration.RegistrationDetails.PersonalDetails.LastName,
                era_preferredname = registration.RegistrationDetails.PersonalDetails.PreferredName,
                era_initial = registration.RegistrationDetails.PersonalDetails.Initials,
                gendercode = LookupGender(registration.RegistrationDetails.PersonalDetails.Gender),
                birthdate = FromDateTime(DateTime.Parse(registration.RegistrationDetails.PersonalDetails.DateOfBirth)),
                era_collectionandauthorization = registration.RegistrationDetails.InformationCollectionConsent,
                era_sharingrestriction = registration.RegistrationDetails.RestrictedAccess,

                address1_line1 = registration.RegistrationDetails.PrimaryAddress.AddressLine1,
                address1_line2 = registration.RegistrationDetails.PrimaryAddress.AddressLine2,
                address1_city = registration.RegistrationDetails.PrimaryAddress.Jurisdiction.JurisdictionName,
                address1_country = registration.RegistrationDetails.PrimaryAddress.Country.CountryCode,
                era_City = Lookup(registration.RegistrationDetails.PrimaryAddress.Jurisdiction),
                era_ProvinceState = Lookup(registration.RegistrationDetails.PrimaryAddress.StateProvince),
                era_Country = Lookup(registration.RegistrationDetails.PrimaryAddress.Country),
                address1_postalcode = registration.RegistrationDetails.PrimaryAddress.PostalCode,

                address2_line1 = registration.RegistrationDetails.MailingAddress.AddressLine1,
                address2_line2 = registration.RegistrationDetails.MailingAddress.AddressLine2,
                address2_city = registration.RegistrationDetails.MailingAddress.Jurisdiction.JurisdictionName,
                address2_country = registration.RegistrationDetails.MailingAddress.Country.CountryName,
                era_MailingCity = Lookup(registration.RegistrationDetails.MailingAddress.Jurisdiction),
                era_MailingProvinceState = Lookup(registration.RegistrationDetails.MailingAddress.StateProvince),
                era_MailingCountry = Lookup(registration.RegistrationDetails.MailingAddress.Country),
                address2_postalcode = registration.RegistrationDetails.MailingAddress.PostalCode,

                emailaddress1 = registration.RegistrationDetails.ContactDetails.Email,
                address1_telephone1 = registration.RegistrationDetails.ContactDetails.Phone,

                era_phonenumberrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Phone),
                era_emailrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Email),
                era_secrettext = registration.RegistrationDetails.SecretPhrase
            };

            // members
            var members = (registration.PreliminaryNeedsAssessment.FamilyMembers ?? Array.Empty<PersonDetails>()).Select(fm => new contact
            {
                contactid = Guid.NewGuid(),
                era_registranttype = 174360001,
                era_authenticated = false,
                era_verified = false,
                era_registrationdate = now,
                firstname = fm.FirstName,
                lastname = fm.LastName,
                era_preferredname = fm.PreferredName,
                era_initial = fm.Initials,
                gendercode = LookupGender(fm.Gender),
                birthdate = FromDateTime(DateTime.Parse(fm.DateOfBirth)),
                era_collectionandauthorization = registration.RegistrationDetails.InformationCollectionConsent,
                era_sharingrestriction = registration.RegistrationDetails.RestrictedAccess,

                address1_line1 = registration.RegistrationDetails.PrimaryAddress.AddressLine1,
                address1_line2 = registration.RegistrationDetails.PrimaryAddress.AddressLine2,
                address1_city = registration.RegistrationDetails.PrimaryAddress.Jurisdiction.JurisdictionName,
                address1_country = registration.RegistrationDetails.PrimaryAddress.Country.CountryCode,
                era_City = Lookup(registration.RegistrationDetails.PrimaryAddress.Jurisdiction),
                era_ProvinceState = Lookup(registration.RegistrationDetails.PrimaryAddress.StateProvince),
                era_Country = Lookup(registration.RegistrationDetails.PrimaryAddress.Country),
                address1_postalcode = registration.RegistrationDetails.PrimaryAddress.PostalCode,

                address2_line1 = registration.RegistrationDetails.MailingAddress.AddressLine1,
                address2_line2 = registration.RegistrationDetails.MailingAddress.AddressLine2,
                address2_city = registration.RegistrationDetails.MailingAddress.Jurisdiction.JurisdictionName,
                address2_country = registration.RegistrationDetails.MailingAddress.Country.CountryName,
                era_MailingCity = Lookup(registration.RegistrationDetails.MailingAddress.Jurisdiction),
                era_MailingProvinceState = Lookup(registration.RegistrationDetails.MailingAddress.StateProvince),
                era_MailingCountry = Lookup(registration.RegistrationDetails.MailingAddress.Country),
                address2_postalcode = registration.RegistrationDetails.MailingAddress.PostalCode,

                emailaddress1 = registration.RegistrationDetails.ContactDetails.Email,
                address1_telephone1 = registration.RegistrationDetails.ContactDetails.Phone,

                era_phonenumberrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Phone),
                era_emailrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Email),
                era_secrettext = registration.RegistrationDetails.SecretPhrase
            });

            // needs assessment
            var needsAssessment = new era_needassessment
            {
                era_needassessmentid = Guid.NewGuid(),
                era_needsassessmentdate = now,
                era_EvacuationFile = file,
                era_needsassessmenttype = 174360000,
                era_foodrequirement = Lookup(registration.PreliminaryNeedsAssessment.RequiresFood),
                era_clothingrequirement = Lookup(registration.PreliminaryNeedsAssessment.RequiresClothing),
                era_dietaryrequirement = registration.PreliminaryNeedsAssessment.HaveSpecialDiet,
                era_incidentalrequirement = Lookup(registration.PreliminaryNeedsAssessment.RequiresIncidentals),
                era_lodgingrequirement = Lookup(registration.PreliminaryNeedsAssessment.RequiresLodging),
                era_transportationrequirement = Lookup(registration.PreliminaryNeedsAssessment.RequiresTransportation),
                era_medicationrequirement = registration.PreliminaryNeedsAssessment.HaveMedication,
                era_insurancecoverage = Lookup(registration.PreliminaryNeedsAssessment.Insurance),
                era_collectionandauthorization = registration.RegistrationDetails.InformationCollectionConsent,
                era_sharingrestriction = registration.RegistrationDetails.RestrictedAccess,
                era_phonenumberrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Phone),
                era_emailrefusal = string.IsNullOrEmpty(registration.RegistrationDetails.ContactDetails.Email)
            };

            // pets
            var pets = (registration.PreliminaryNeedsAssessment.Pets ?? Array.Empty<Pet>()).Select(p => new era_evacuee
            {
                era_evacueeid = Guid.NewGuid(),
                era_needsassessment = needsAssessment,
                era_amountofpets = Convert.ToInt32(p.Quantity),
                era_typeofpet = p.Type
            });

            // set enity data and entity links in Dynamics

            // save evacuation file to dynamics
            dynamicsClient.AddToera_evacuationfiles(file);
            // save needs assessment to dynamics
            dynamicsClient.AddToera_needassessments(needsAssessment);
            // link evacuation file to needs assessment
            dynamicsClient.AddLink(file, nameof(file.era_needsassessment_EvacuationFile), needsAssessment);

            // save registrant to dynamics
            dynamicsClient.AddTocontacts(registrant);
            var evacueeRegistrant = new era_evacuee
            {
                era_evacueeid = Guid.NewGuid(),
                era_needsassessment = needsAssessment,
                era_Registrant = registrant
            };
            dynamicsClient.AddToera_evacuees(evacueeRegistrant);
            // link registrant and needs assessment to evacuee record
            dynamicsClient.AddLink(registrant, nameof(registrant.era_contact_evacuee_Registrant), evacueeRegistrant);
            dynamicsClient.AddLink(needsAssessment, nameof(needsAssessment.era_era_needassessment_era_evacuee_needsassessment), evacueeRegistrant);

            // save members to dynamics
            foreach (var member in members)
            {
                dynamicsClient.AddTocontacts(member);
                var evacueeMember = new era_evacuee
                {
                    era_evacueeid = Guid.NewGuid(),
                    era_needsassessment = needsAssessment,
                    era_Registrant = member
                };
                dynamicsClient.AddToera_evacuees(evacueeMember);
                // link members and needs assessment to evacuee record
                dynamicsClient.AddLink(member, nameof(member.era_contact_evacuee_Registrant), evacueeMember);
                dynamicsClient.AddLink(needsAssessment, nameof(needsAssessment.era_era_needassessment_era_evacuee_needsassessment), evacueeMember);
            }

            // save pets to dynamics
            foreach (var pet in pets)
            {
                var petMember = new era_evacuee
                {
                    era_evacueeid = Guid.NewGuid(),
                    era_needsassessment = needsAssessment,
                    era_typeofpet = pet.era_typeofpet,
                    era_amountofpets = pet.era_amountofpets
                };
                dynamicsClient.AddToera_evacuees(petMember);

                try
                {
                    // link pet to evacuee record
                    dynamicsClient.AddLink(needsAssessment, nameof(needsAssessment.era_era_needassessment_era_evacuee_needsassessment), petMember);
                }
                catch (ArgumentNullException)
                {
                    logger.LogError("ArgumentNullException linking entities");
                    throw;
                }
                catch (InvalidOperationException)
                {
                    logger.LogError("InvalidOperationException linking entities");
                    throw;
                }
            }

            //post as batch is not accepted by SSG. Sending with default option (multiple requests to the server stopping on the first failure)
            //var results = await dynamicsClient.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
            var results = await dynamicsClient.SaveChangesAsync(SaveChangesOptions.ContinueOnError);

            //var newEvacuationFileId = ((era_evacuationfile)results
            //    .Select(r => (EntityDescriptor)((ChangeOperationResponse)r).Descriptor)
            //    .Single(ed => ed.Entity is era_evacuationfile).Entity).era_evacuationfileid;

            //var essFileNumber = dynamicsClient.era_evacuationfiles
            //    .Where(ef => ef.era_evacuationfileid == newEvacuationFileId)
            //    .Single().era_essfilenumber;

            return $"E{essFileNumber:D9}";
        }

        private era_country Lookup(Country country) =>
            string.IsNullOrEmpty(country.CountryCode)
            ? null
            : dynamicsClient.era_countries.Where(c => c.era_countrycode == country.CountryCode).FirstOrDefault();

        private int Lookup(bool? value) => value.HasValue ? value.Value ? 174360000 : 174360001 : 174360002;

        private int? Lookup(NeedsAssessment.InsuranceOption value) => value switch
        {
            NeedsAssessment.InsuranceOption.No => 174360000,
            NeedsAssessment.InsuranceOption.Yes => 174360001,
            NeedsAssessment.InsuranceOption.Unsure => 174360002,
            NeedsAssessment.InsuranceOption.Unknown => 174360003,
            _ => null
        };

        private int? LookupGender(string value) => value switch
        {
            "M" => 1,
            "F" => 2,
            "X" => 3,
            _ => null
        };

        private era_provinceterritories Lookup(StateProvince stateProvince) =>
        string.IsNullOrEmpty(stateProvince.StateProvinceCode)
            ? null
            : dynamicsClient.era_provinceterritorieses.Where(p => p.era_code == stateProvince.StateProvinceCode).FirstOrDefault();

        private era_jurisdiction Lookup(Jurisdiction jurisdiction) =>
            string.IsNullOrEmpty(jurisdiction.JurisdictionCode)
            ? null
            : dynamicsClient.era_jurisdictions.Where(j => j.era_jurisdictionid == Guid.Parse(jurisdiction.JurisdictionCode)).FirstOrDefault();

        //private int Lookup(string entityName, string optionSetName, string label) =>
        //    dynamicsClient.Execute<AttributeMetadata>(new Uri($"EntityDefinitions(LogicalName='{entityName}')/Attributes(LogicalName='{optionSetName}')"))
        //    .Cast<OptionSetMetadata>()
        //    .Single().Options.Single(o => o.ExternalValue == label).Value.Value;

        private Date? FromDateTime(DateTime? dateTime) => dateTime.HasValue ? new Date(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day) : (Date?)null;
    }
}
