import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OrganizationDetailComponent } from './organization-detail.component';
import { OrganizationService } from '../../services/organization';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { of, throwError } from 'rxjs';
import { OrganizationSummary } from '../../models';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('OrganizationDetailComponent', () => {
  let component: OrganizationDetailComponent;
  let fixture: ComponentFixture<OrganizationDetailComponent>;
  let mockOrganizationService: any;
  let mockActivatedRoute: any;

  beforeEach(async () => {
    mockOrganizationService = {
      getOrganizationSummary: vi.fn()
    };

    mockActivatedRoute = {
      params: of({ id: '1' })
    };

    await TestBed.configureTestingModule({
      imports: [OrganizationDetailComponent, RouterModule.forRoot([])],
      providers: [
        { provide: OrganizationService, useValue: mockOrganizationService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    const mockSummary: OrganizationSummary = {
      organization: {
        id: '1',
        name: 'Tech Corp',
        industry: 'Technology',
        tier: 'enterprise',
        contactEmail: 'contact@techcorp.com'
      },
      projectCount: 5,
      userCount: 15,
      totalInvoiced: 250000,
      currency: 'USD'
    };

    mockOrganizationService.getOrganizationSummary.mockReturnValue(of(mockSummary));
    
    fixture = TestBed.createComponent(OrganizationDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    
    expect(component).toBeTruthy();
  });

  it('should load organization summary successfully', () => {
    const mockSummary: OrganizationSummary = {
      organization: {
        id: '1',
        name: 'Tech Corp',
        industry: 'Technology',
        tier: 'enterprise',
        contactEmail: 'contact@techcorp.com'
      },
      projectCount: 5,
      userCount: 15,
      totalInvoiced: 250000,
      currency: 'USD'
    };

    mockOrganizationService.getOrganizationSummary.mockReturnValue(of(mockSummary));
    
    fixture = TestBed.createComponent(OrganizationDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toEqual(mockSummary);
    expect(component.state().loading).toBe(false);
    expect(component.state().error).toBeNull();
  });

  it('should handle error when loading organization', () => {
    mockOrganizationService.getOrganizationSummary.mockReturnValue(
      throwError(() => new Error('Not found'))
    );
    
    fixture = TestBed.createComponent(OrganizationDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toBeNull();
    expect(component.state().error).toBeTruthy();
  });

  it('should display back link to organizations', () => {
    const mockSummary: OrganizationSummary = {
      organization: {
        id: '1',
        name: 'Tech Corp',
        industry: 'Technology',
        tier: 'enterprise',
        contactEmail: 'contact@techcorp.com'
      },
      projectCount: 5,
      userCount: 15,
      totalInvoiced: 250000,
      currency: 'USD'
    };

    mockOrganizationService.getOrganizationSummary.mockReturnValue(of(mockSummary));
    
    fixture = TestBed.createComponent(OrganizationDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    const backLink = compiled.querySelector('a[routerLink="/organizations"]');
    expect(backLink).toBeTruthy();
  });
});
