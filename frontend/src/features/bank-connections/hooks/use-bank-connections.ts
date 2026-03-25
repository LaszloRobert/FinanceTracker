import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { api } from '@/lib/api';
import type { BankConnection, Institution } from '../types';

export function useBankConnections() {
  return useQuery({
    queryKey: ['bank-connections'],
    queryFn: () => api.get<BankConnection[]>('/bank-connections'),
  });
}

export function useInstitutions(countryCode: string, enabled: boolean) {
  return useQuery({
    queryKey: ['institutions', countryCode],
    queryFn: () =>
      api.get<Institution[]>(`/bank-connections/institutions?countryCode=${countryCode}`),
    enabled,
  });
}

export function useConnectBank() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (institutionId: string) =>
      api.post<{ requisitionId: string; authLink: string }>('/bank-connections/connect', {
        institutionId,
        redirectUrl: `${window.location.origin}/bank-connections/callback`,
      }),
    onSuccess: (data) => {
      window.open(data.authLink, '_blank');
      void queryClient.invalidateQueries({ queryKey: ['bank-connections'] });
      toast.success('Bank connection initiated. Complete authorization in the new tab.');
    },
    onError: () => {
      toast.error('Failed to connect bank');
    },
  });
}

export function useProcessCallback() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (requisitionId: string) =>
      api.post<{ bankConnectionId: string }>('/bank-connections/callback', { requisitionId }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['bank-connections'] });
      void queryClient.invalidateQueries({ queryKey: ['accounts'] });
      toast.success('Bank connected successfully!');
    },
    onError: () => {
      toast.error('Failed to process bank connection');
    },
  });
}
