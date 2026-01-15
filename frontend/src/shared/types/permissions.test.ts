import { describe, it, expect } from 'vitest'
import {
  Permission,
  PERMISSIONS,
  ROLES,
  MODULE_IDS,
  DEFAULT_ROLE_PERMISSIONS,
  DEFAULT_ROLE_MODULES,
  type Role,
  type ModuleId,
} from './permissions.types'

describe('permissions.types', () => {
  describe('Permission', () => {
    it('should have all expected permission values', () => {
      expect(Permission.CheckPayrollCompliance).toBe('CheckPayrollCompliance')
      expect(Permission.CheckRosterCompliance).toBe('CheckRosterCompliance')
      expect(Permission.ManageDocuments).toBe('ManageDocuments')
      expect(Permission.ViewAllPayroll).toBe('ViewAllPayroll')
      expect(Permission.ViewOwnPayslip).toBe('ViewOwnPayslip')
    })

    it('should have PERMISSIONS array containing all permission values', () => {
      expect(PERMISSIONS).toContain(Permission.CheckPayrollCompliance)
      expect(PERMISSIONS).toContain(Permission.CheckRosterCompliance)
      expect(PERMISSIONS).toContain(Permission.ManageDocuments)
      expect(PERMISSIONS.length).toBe(13)
    })
  })

  describe('ROLES', () => {
    it('should contain all valid roles', () => {
      expect(ROLES).toContain('admin')
      expect(ROLES).toContain('manager')
      expect(ROLES).toContain('staff')
      expect(ROLES.length).toBe(3)
    })
  })

  describe('MODULE_IDS', () => {
    it('should contain all module identifiers', () => {
      expect(MODULE_IDS).toContain('payroll')
      expect(MODULE_IDS).toContain('roster')
      expect(MODULE_IDS).toContain('documents')
      expect(MODULE_IDS).toContain('settings')
      expect(MODULE_IDS).toContain('fairbot')
      expect(MODULE_IDS).toContain('employees')
      expect(MODULE_IDS.length).toBe(6)
    })
  })

  describe('DEFAULT_ROLE_PERMISSIONS', () => {
    it('should give admin all permissions', () => {
      const adminPerms = DEFAULT_ROLE_PERMISSIONS.admin
      expect(adminPerms).toContain(Permission.CheckPayrollCompliance)
      expect(adminPerms).toContain(Permission.CheckRosterCompliance)
      expect(adminPerms).toContain(Permission.ManageDocuments)
      expect(adminPerms).toContain(Permission.ManageAllEmployees)
    })

    it('should give manager limited permissions', () => {
      const managerPerms = DEFAULT_ROLE_PERMISSIONS.manager
      expect(managerPerms).toContain(Permission.CheckRosterCompliance)
      expect(managerPerms).not.toContain(Permission.CheckPayrollCompliance)
      expect(managerPerms).not.toContain(Permission.ManageDocuments)
    })

    it('should give staff minimal permissions', () => {
      const staffPerms = DEFAULT_ROLE_PERMISSIONS.staff
      expect(staffPerms).toContain(Permission.ViewOwnPayslip)
      expect(staffPerms).toContain(Permission.ViewOwnProfile)
      expect(staffPerms).not.toContain(Permission.CheckPayrollCompliance)
      expect(staffPerms).not.toContain(Permission.CheckRosterCompliance)
    })

    it('should have permissions defined for all roles', () => {
      const roles: Role[] = ['admin', 'manager', 'staff']
      roles.forEach((role) => {
        expect(DEFAULT_ROLE_PERMISSIONS[role]).toBeDefined()
        expect(Array.isArray(DEFAULT_ROLE_PERMISSIONS[role])).toBe(true)
      })
    })
  })

  describe('DEFAULT_ROLE_MODULES', () => {
    it('should give admin access to all modules', () => {
      const adminModules = DEFAULT_ROLE_MODULES.admin
      expect(adminModules).toContain('payroll')
      expect(adminModules).toContain('roster')
      expect(adminModules).toContain('documents')
      expect(adminModules).toContain('settings')
      expect(adminModules).toContain('fairbot')
      expect(adminModules).toContain('employees')
    })

    it('should give manager limited module access', () => {
      const managerModules = DEFAULT_ROLE_MODULES.manager
      expect(managerModules).toContain('roster')
      expect(managerModules).toContain('fairbot')
      expect(managerModules).not.toContain('payroll')
      expect(managerModules).not.toContain('settings')
    })

    it('should give staff minimal module access', () => {
      const staffModules = DEFAULT_ROLE_MODULES.staff
      expect(staffModules).toContain('fairbot')
      expect(staffModules).not.toContain('payroll')
      expect(staffModules).not.toContain('roster')
      expect(staffModules).not.toContain('settings')
    })

    it('should have modules defined for all roles', () => {
      const roles: Role[] = ['admin', 'manager', 'staff']
      roles.forEach((role) => {
        expect(DEFAULT_ROLE_MODULES[role]).toBeDefined()
        expect(Array.isArray(DEFAULT_ROLE_MODULES[role])).toBe(true)
      })
    })
  })
})
