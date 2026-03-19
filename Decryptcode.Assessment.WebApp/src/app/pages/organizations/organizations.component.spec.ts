import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OrganizationsComponent } from './organizations.component';
import { OrganizationService } from '../../services/organization';
import { RouterModule } from '@angular/router';
import { of, throwError } from 'rxjs';
import { Organization } from '../../models';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('OrganizationsComponent', () => {
  let component: OrganizationsComponent;
  let fixture: ComponentFixture<OrganizationsComponent>;
  let mockOrganizationService: any;

  beforeEach(async () => {
    mockOrganizationService = {
      getOrganizations: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [OrganizationsComponent, RouterModule.forRoot([])],
      providers: [
        { provide: OrganizationService, useValue: mockOrganizationService }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    const mockOrgs: Organization[] = [];
    mockOrganizationService.getOrganizations.mockReturnValue(of(mockOrgs));
    
    fixture = TestBed.createComponent(OrganizationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    
    expect(component).toBeTruthy();
  });

  it('should load organizations successfully', () => {
    const mockOrganizations: Organization[] = [
      {
        id: '1',
        name: 'Tech Corp',
        industry: 'Technology',
        tier: 'enterprise',
        contactEmail: 'contact@techcorp.com'
      }
    ];

    mockOrganizationService.getOrganizations.mockReturnValue(of(mockOrganizations));
    
    fixture = TestBed.createComponent(OrganizationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toEqual(mockOrganizations);
    expect(component.state().loading).toBe(false);
    expect(component.state().error).toBeNull();
  });

  it('should handle error when loading organizations', () => {
    mockOrganizationService.getOrganizations.mockReturnValue(
      throwError(() => new Error('Network error'))
    );
    
    fixture = TestBed.createComponent(OrganizationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toBeNull();
    expect(component.state().error).toBeTruthy();
  });

  it('should display organizations title in template', () => {
    const mockOrganizations: Organization[] = [];
    mockOrganizationService.getOrganizations.mockReturnValue(of(mockOrganizations));
    
    fixture = TestBed.createComponent(OrganizationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Organizations');
  });
});
