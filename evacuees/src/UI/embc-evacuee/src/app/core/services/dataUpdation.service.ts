import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { RegAddress } from '../model/address';
import { DataService } from './data.service';
import { FormCreationService } from './formCreation.service';

@Injectable({ providedIn: 'root' })
export class DataUpdationService {

    constructor(public dataService: DataService, public formCreationService: FormCreationService) { }

    updatePersonalDetails(formGroup: FormGroup) {
        this.formCreationService.setPersonDetailsForm(formGroup);
        this.dataService.updateRegistartion({ personalDetails: formGroup.value });
    }

    updateAddressDetails(formGroup: FormGroup) {
        this.formCreationService.setAddressForm(formGroup);
        this.dataService.updateRegistartion({ mailingAddress: this.setAddressObject(formGroup.get('mailingAddress').value) });
        this.dataService.updateRegistartion({ primaryAddress: this.setAddressObject(formGroup.get('address').value) });
    }

    updateContactDetails(formGroup: FormGroup) {
        this.formCreationService.setContactDetailsForm(formGroup);
        this.dataService.updateRegistartion({ contactDetails: formGroup.value });
    }

    updateSecretDetails(formGroup: FormGroup) {
        this.formCreationService.setSecretForm(formGroup);
        this.dataService.updateRegistartion(formGroup.value);
    }

    updateEvacuationDetails(formGroup: FormGroup) {
        this.formCreationService.setEvacuatedForm(formGroup);
        this.dataService.updateNeedsAssessment({ evacuatedFromAddress: this.setAddressObject(formGroup.get('evacuatedFromAddress').value) });
        this.dataService.updateNeedsAssessment({ insurance: formGroup.get('insurance').value });
    }

    updateFamilyMemberDetails(formGroup: FormGroup) {
        this.formCreationService.setFamilyMembersForm(formGroup);
        this.dataService.updateNeedsAssessment({ haveMedication: formGroup.get('haveMedication').value });
        this.dataService.updateNeedsAssessment({ haveSpecialDiet: formGroup.get('haveSpecialDiet').value });
        this.dataService.updateNeedsAssessment({ familyMembers: formGroup.get('familyMember').value });
    }

    updatePetsDetails(formGroup: FormGroup){
        this.formCreationService.setPetsForm(formGroup);
        this.dataService.updateNeedsAssessment({ pets: formGroup.get('pets').value });
        this.dataService.updateNeedsAssessment({ hasPetsFood: formGroup.get('hasPetsFood').value })
    }

    updateNeedsDetails(formGroup: FormGroup) {
        this.formCreationService.setIdentifyNeedsForm(formGroup);
        this.dataService.updateNeedsAssessment({ requiresClothing: formGroup.get('requiresClothing').value === "null" ? null : formGroup.get('requiresClothing').value });
        this.dataService.updateNeedsAssessment({ requiresFood: formGroup.get('requiresFood').value === "null" ? null : formGroup.get('requiresFood').value });
        this.dataService.updateNeedsAssessment({ requiresIncidentals: formGroup.get('requiresIncidentals').value === "null" ? null : formGroup.get('requiresIncidentals').value });
        this.dataService.updateNeedsAssessment({ requiresLodging: formGroup.get('requiresLodging').value === "null" ? null : formGroup.get('requiresLodging').value });
        this.dataService.updateNeedsAssessment({ requiresTransportation: formGroup.get('requiresTransportation').value === "null" ? null : formGroup.get('requiresTransportation').value });
    }

    private setAddressObject(addressObject): RegAddress {
        let address: RegAddress = {
            addressLine1: addressObject.addressLine1,
            addressLine2: addressObject.addressLine2,
            country: {
                countryCode: addressObject.country.code,
                countryName: addressObject.country.name
            },
            jurisdiction: {
                jurisdictionCode: addressObject.jurisdiction.code === undefined ? null : addressObject.jurisdiction.code,
                jurisdictionName: addressObject.jurisdiction.name === undefined ? addressObject.jurisdiction : addressObject.jurisdiction.name
            },
            postalCode: addressObject.postalCode,
            stateProvince: {
                stateProvinceCode: addressObject.stateProvince.code === undefined ? null : addressObject.stateProvince.code,
                stateProvinceName: addressObject.stateProvince.name === undefined ? addressObject.stateProvince : addressObject.stateProvince.name
            }
        }

        return address;
    }
}
