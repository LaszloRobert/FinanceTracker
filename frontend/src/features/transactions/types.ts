export interface Transaction {
  id: string;
  accountId: string;
  amount: number;
  currency: string;
  bookingDate: string | null;
  valueDate: string | null;
  creditorName: string | null;
  debtorName: string | null;
  remittanceInfo: string | null;
  status: string;
  categoryId: string | null;
  categoryName: string | null;
}

export interface TransactionsResponse {
  items: Transaction[];
  totalCount: number;
  page: number;
  pageSize: number;
}
