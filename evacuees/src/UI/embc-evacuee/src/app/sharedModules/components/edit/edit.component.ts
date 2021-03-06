import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { Subscription } from 'rxjs';
import { DataUpdationService } from 'src/app/core/services/dataUpdation.service';
import { FormCreationService } from 'src/app/core/services/formCreation.service';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditComponent implements OnInit {
  componentToLoad: string;
  profileFolderPath: string;
  navigationExtras: NavigationExtras = { state: { stepIndex: 4 } };
  form$: Subscription;
  form: FormGroup;
  editHeading: string;
  currentFlow: string;
  parentPageName: string;
  nonVerfiedRoute = '/non-verified-registration/needs-assessment';
  verifiedRoute = '/verified-registration/create-profile';

  constructor(private router: Router, private route: ActivatedRoute, public updateService: DataUpdationService,
              private formCreationService: FormCreationService) {
    const navigation = this.router.getCurrentNavigation();
    if (navigation.extras.state !== undefined) {
      const state = navigation.extras.state as { parentPageName: string };
      this.parentPageName = state.parentPageName;
      console.log(this.parentPageName);
    }
  }

  ngOnInit(): void {
    this.currentFlow = this.route.snapshot.data.flow;
    this.route.paramMap.subscribe(params => {
      this.componentToLoad = params.get('type');
      this.loadForm(this.componentToLoad);
    });
  }

  save(): void {
    this.setFormData(this.componentToLoad);
    if (this.currentFlow === 'non-verified-registration') {
      this.router.navigate([this.nonVerfiedRoute], this.navigationExtras);
    } else {
      if (this.parentPageName === 'create-profile') {
        this.router.navigate([this.verifiedRoute], this.navigationExtras);
      } else if (this.parentPageName === 'view-profile') {
        this.router.navigate(['/verified-registration/view-profile']);
      }
    }
  }

  cancel(): void {
    if (this.currentFlow === 'non-verified-registration') {
      this.router.navigate([this.nonVerfiedRoute], this.navigationExtras);
    } else {
      if (this.parentPageName === 'create-profile') {
        this.router.navigate([this.verifiedRoute], this.navigationExtras);
      } else if (this.parentPageName === 'view-profile') {
        this.router.navigate(['/verified-registration/view-profile']);
      }
    }
  }

  setFormData(component: string): void {
    switch (component) {
      case 'restriction':
        this.updateService.updateRestriction(this.form);
        break;
      case 'personal-details':
        this.updateService.updatePersonalDetails(this.form);
        break;
      case 'address':
        this.updateService.updateAddressDetails(this.form);
        break;
      case 'contact-info':
        this.updateService.updateContactDetails(this.form);
        break;
      case 'secret':
        this.updateService.updateSecretDetails(this.form);
        break;
      case 'evac-address':
        this.updateService.updateEvacuationDetails(this.form);
        break;
      case 'family-information':
        this.updateService.updateFamilyMemberDetails(this.form);
        break;
      case 'pets':
        this.updateService.updatePetsDetails(this.form);
        break;
      case 'identify-needs':
        this.updateService.updateNeedsDetails(this.form);
        break;
      default:
    }
  }

  loadForm(component: string): void {
    switch (component) {
      case 'restriction':
        this.form$ = this.formCreationService.getRestrictionForm().subscribe(
          restriction => {
            this.form = restriction;
          }
        );
        this.editHeading = 'Edit Restriction';
        break;
      case 'personal-details':
        this.form$ = this.formCreationService.getPeronalDetailsForm().subscribe(
          personalDetails => {
            this.form = personalDetails;
          }
        );
        this.editHeading = 'Edit Profile';
        this.profileFolderPath = 'evacuee-profile-forms';
        break;
      case 'address':
        this.form$ = this.formCreationService.getAddressForm().subscribe(
          address => {
            this.form = address;
          }
        );
        this.editHeading = 'Edit Profile';
        this.profileFolderPath = 'evacuee-profile-forms';
        break;
      case 'contact-info':
        this.form$ = this.formCreationService.getContactDetailsForm().subscribe(
          contactDetails => {
            this.form = contactDetails;
          }
        );
        this.editHeading = 'Edit Profile';
        this.profileFolderPath = 'evacuee-profile-forms';
        break;
      case 'secret':
        this.form$ = this.formCreationService.getSecretForm().subscribe(
          secret => {
            this.form = secret;
          }
        );
        this.editHeading = 'Edit Profile';
        this.profileFolderPath = 'evacuee-profile-forms';
        break;
      case 'evac-address':
        this.form$ = this.formCreationService.getEvacuatedForm().subscribe(
          evacuatedForm => {
            this.form = evacuatedForm;
          }
        );
        this.editHeading = 'Edit Evacuation File';
        this.profileFolderPath = 'needs-assessment-forms';
        break;
      case 'family-information':
        this.form$ = this.formCreationService.getFamilyMembersForm().subscribe(
          memberForm => {
            this.form = memberForm;
          }
        );
        this.editHeading = 'Edit Evacuation File';
        this.profileFolderPath = 'needs-assessment-forms';
        break;
      case 'pets':
        this.form$ = this.formCreationService.getPetsForm().subscribe(
          petsForm => {
            this.form = petsForm;
          }
        );
        this.editHeading = 'Edit Evacuation File';
        this.profileFolderPath = 'needs-assessment-forms';
        break;
      case 'identify-needs':
        this.form$ = this.formCreationService.getIndentifyNeedsForm().subscribe(
          identifyNeedsForm => {
            this.form = identifyNeedsForm;
          }
        );
        this.editHeading = 'Edit Evacuation File';
        this.profileFolderPath = 'needs-assessment-forms';
        break;
      default:
    }
  }

}
