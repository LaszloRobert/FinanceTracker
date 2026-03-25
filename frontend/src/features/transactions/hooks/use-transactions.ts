import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { TransactionsResponse } from '../types';

interface TransactionFilters {
  accountId?: string;
  categoryId?: string;
  dateFrom?: string;
  dateTo?: string;
  page: number;
  pageSize: number;
}

export function useTransactions(filters: TransactionFilters) {
  const params = new URLSearchParams();
  params.set('page', String(filters.page));
  params.set('pageSize', String(filters.pageSize));
  if (filters.accountId) params.set('accountId', filters.accountId);
  if (filters.categoryId) params.set('categoryId', filters.categoryId);
  if (filters.dateFrom) params.set('dateFrom', filters.dateFrom);
  if (filters.dateTo) params.set('dateTo', filters.dateTo);

  return useQuery({
    queryKey: ['transactions', filters],
    queryFn: () => api.get<TransactionsResponse>(`/transactions?${params.toString()}`),
  });
}
