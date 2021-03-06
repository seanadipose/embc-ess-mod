import { Component, OnInit, Input, Injector } from '@angular/core';
import { from } from 'rxjs';
import { FormBuilder } from '@angular/forms';
import { FormCreationService } from '../../../core/services/formCreation.service';

@Component({
  selector: 'app-component-wrapper',
  templateUrl: './component-wrapper.component.html',
  styleUrls: ['./component-wrapper.component.scss']
})
export class ComponentWrapperComponent implements OnInit {

  @Input() componentName: string;
  @Input() folderPath: string;
  loadedComponent: any;
  serviceInjector: Injector;

  constructor(private injector: Injector, private formBuilder: FormBuilder, private formCreationService: FormCreationService) { }

  ngOnInit(): void {
    if (!this.loadedComponent) {
      this.serviceInjector = Injector.create({
        providers: [{
          provide: 'formBuilder',
          useValue: this.formBuilder
        },
        {
          provide: 'formCreationService',
          useValue: this.formCreationService
        }],
        parent: this.injector
      });
    }
    from(this.loadComponent()).subscribe(module => {
      this.loadedComponent = module.default;
    });
  }

  loadComponent(): Promise<any> {
    return Promise.resolve(import(`../../forms/${this.folderPath}/${this.componentName}/${this.componentName}.component`));
  }
  // ../core/components/evacuee-profile-forms/${this.componentName}/${this.componentName}.component`
}
