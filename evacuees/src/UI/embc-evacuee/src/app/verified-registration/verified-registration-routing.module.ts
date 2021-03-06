import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { VerifiedRegistrationComponent } from './verified-registration.component';

const routes: Routes = [
  {
    path: '', component: VerifiedRegistrationComponent,
    children: [
      {
        path: '',
        redirectTo: 'collection-notice',
        pathMatch: 'full',
      },
      {
        path: 'collection-notice',
        loadChildren: () => import('../sharedModules/components/collection-notice/collection-notice.module')
          .then(m => m.CollectionNoticeModule),
        data: { flow: 'verified-registration' }
      },
      {
        path: 'restriction',
        loadChildren: () => import('../sharedModules/components/restriction/restriction.module').then(m => m.RestrictionModule),
        data: { flow: 'verified-registration' }
      },
      {
        path: 'create-profile',
        loadChildren: () => import('../sharedModules/components/profile/profile.module').then(m => m.ProfileModule),
        data: { flow: 'verified-registration' }
      },
      {
        path: 'view-profile',
        loadChildren: () => import('../sharedModules/components/view-auth-profile/view-auth-profile.module')
          .then(m => m.ViewAuthProfileModule),
        data: { flow: 'verified-registration' }
      },
      {
        path: 'edit/:type',
        loadChildren: () => import('../sharedModules/components/edit/edit.module').then(m => m.EditModule),
        data: { flow: 'verified-registration' }
      },
      {
        path: 'confirm-restriction',
        loadChildren: () => import('../sharedModules/components/confirm-restriction/confirm-restriction.module')
          .then(m => m.ConfirmRestrictionModule),
        data: { flow: 'verified-registration' }
      },
      {
        path: 'needs-assessment',
        loadChildren: () => import('../sharedModules/components/needs-assessment/needs-assessment.module')
          .then(m => m.NeedsAssessmentModule),
        data: { flow: 'verified-registration' }
      },
      {
        path: 'fileSubmission',
        loadChildren: () => import('../sharedModules/components/file-submission/file-submission.module')
          .then(m => m.FileSubmissionModule),
        data: { flow: 'verified-registration' }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VerifiedRegistrationRoutingModule { }
