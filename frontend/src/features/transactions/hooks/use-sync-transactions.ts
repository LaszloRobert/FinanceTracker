import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { api } from '@/lib/api';

export function useSyncAccount() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (accountId: string) =>
      api.post<{ syncedCount: number }>(`/transactions/${accountId}/sync`),
    onSuccess: (data) => {
      void queryClient.invalidateQueries({ queryKey: ['transactions'] });
      void queryClient.invalidateQueries({ queryKey: ['accounts'] });
      toast.success(`Synced ${data.syncedCount} new transactions`);
    },
    onError: () => {
      toast.error('Failed to sync transactions');
    },
  });
}

export function useSyncAll() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () =>
      api.post<{ syncedCount: number }>('/transactions/sync-all'),
    onSuccess: (data) => {
      void queryClient.invalidateQueries({ queryKey: ['transactions'] });
      void queryClient.invalidateQueries({ queryKey: ['accounts'] });
      toast.success(`Synced ${data.syncedCount} new transactions across all accounts`);
    },
    onError: () => {
      toast.error('Failed to sync transactions');
    },
  });
}
