export interface DashboardData {
  totalOrganizations: number;
  totalUsers: number;
  totalProjects: number;
  activeProjects: number;
  totalTimeEntries: number;
  totalInvoiced: number;
}

export interface Organization {
  id: string;
  name: string;
  industry: string;
  tier: 'starter' | 'professional' | 'enterprise';
  contactEmail: string;
}

export interface OrganizationSummary {
  organization: Organization;
  projectCount: number;
  userCount: number;
  totalInvoiced: number;
  currency: string;
}

export interface Project {
  id: string;
  name: string;
  status: 'active' | 'completed' | 'draft';
  budgetHours: number;
  startDate?: string;
  endDate?: string;
}

export interface ProjectDetail extends Project {
  totalHoursLogged?: number;
  organization?: {
    name: string;
  };
}

export interface User {
  id: string;
  name: string;
  email: string;
}

export interface TimeEntry {
  id: string;
  hours: number;
  date: string;
}

export interface Invoice {
  id: string;
  amount: number;
  date: string;
}
