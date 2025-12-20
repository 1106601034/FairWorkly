/**
 * Uploaded file data structure
 */
export interface UploadedFile {
  id: string;              // Unique ID
  name: string;            // File name
  size: number;            // File size in bytes
  uploadDate: Date;        // Upload timestamp
  status: FileStatus;      // Current status
  file: File;              // Original File object
}

/**
 * File status types
 */
export type FileStatus = 'ready' | 'validating' | 'validated' | 'error';

/**
 * Validation coverage item
 */
export interface ValidationCoverageItem {
  id: string;
  label: string;
  checked: boolean;
}

// File size limit: 10MB
export const MAX_FILE_SIZE = 10 * 1024 * 1024;

// Validation coverage items
export const VALIDATION_COVERAGE_ITEMS: ValidationCoverageItem[] = [
  { id: 'base-rates', label: 'Base rates & award classifications', checked: true },
  { id: 'penalty-rates', label: 'Penalty rates (weekends & public holidays)', checked: true },
  { id: 'casual-loading', label: 'Casual loading (25%)', checked: true },
  { id: 'superannuation', label: 'Superannuation guarantee (12%)', checked: true },
  { id: 'stp-compliance', label: 'Single Touch Payroll (STP) compliance', checked: true }
];