export interface BankConnection {
  id: string;
  institutionId: string;
  institutionName: string;
  institutionLogo: string | null;
  status: string;
  linkedAt: string;
  expiresAt: string;
  accountCount: number;
}

export interface Institution {
  id: string;
  name: string;
  logo: string | null;
  transactionTotalDays: number;
  maxAccessValidForDays: number;
}
