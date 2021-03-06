import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAuthProfileComponent } from './view-auth-profile.component';
import { RouterTestingModule } from '@angular/router/testing';
import { FormCreationService } from '../../../core/services/formCreation.service';

describe('ViewAuthProfileComponent', () => {
  let component: ViewAuthProfileComponent;
  let fixture: ComponentFixture<ViewAuthProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewAuthProfileComponent ],
      imports: [ RouterTestingModule ],
      providers: [ FormCreationService ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewAuthProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
