import { useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { useProcessCallback } from '../hooks/use-bank-connections';
import { Card, CardContent } from '@/components/ui/card';
import { Landmark } from 'lucide-react';

export function BankCallbackPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const processCallback = useProcessCallback();

  const requisitionId = searchParams.get('ref');

  useEffect(() => {
    if (requisitionId && !processCallback.isPending && !processCallback.isSuccess && !processCallback.isError) {
      processCallback.mutate(requisitionId, {
        onSuccess: () => {
          setTimeout(() => navigate('/bank-connections'), 2000);
        },
        onError: () => {
          setTimeout(() => navigate('/bank-connections'), 3000);
        },
      });
    }
  }, [requisitionId]);

  if (!requisitionId) {
    return (
      <div className="flex min-h-[60vh] items-center justify-center">
        <Card className="w-full max-w-sm">
          <CardContent className="flex flex-col items-center py-12">
            <Landmark className="mb-3 size-10 text-destructive" />
            <p className="text-sm font-medium">Invalid callback</p>
            <p className="text-xs text-muted-foreground">No requisition ID found.</p>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="flex min-h-[60vh] items-center justify-center">
      <Card className="w-full max-w-sm">
        <CardContent className="flex flex-col items-center py-12">
          <Landmark className="mb-3 size-10 text-emerald-500" />
          {processCallback.isPending && (
            <>
              <p className="text-sm font-medium">Connecting your bank...</p>
              <p className="text-xs text-muted-foreground">Please wait while we fetch your accounts.</p>
            </>
          )}
          {processCallback.isSuccess && (
            <>
              <p className="text-sm font-medium text-emerald-600">Bank connected successfully!</p>
              <p className="text-xs text-muted-foreground">Redirecting to your connections...</p>
            </>
          )}
          {processCallback.isError && (
            <>
              <p className="text-sm font-medium text-destructive">Connection failed</p>
              <p className="text-xs text-muted-foreground">Redirecting back...</p>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
