import { Wallet, Landmark, ArrowLeftRight } from 'lucide-react';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';

const stats = [
  {
    label: 'Total Accounts',
    value: '--',
    description: 'Connected bank accounts',
    icon: Wallet,
  },
  {
    label: 'Bank Connections',
    value: '--',
    description: 'Active integrations',
    icon: Landmark,
  },
  {
    label: 'Transactions',
    value: '--',
    description: 'Across all accounts',
    icon: ArrowLeftRight,
  },
];

export function DashboardPage() {
  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Dashboard</h1>
        <p className="text-muted-foreground">
          Welcome back! Here is an overview of your finances.
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {stats.map(({ label, value, description, icon: Icon }) => (
          <Card key={label}>
            <CardHeader className="flex flex-row items-center justify-between">
              <div className="space-y-1">
                <CardDescription>{label}</CardDescription>
                <CardTitle className="text-2xl">{value}</CardTitle>
              </div>
              <div className="flex size-10 items-center justify-center rounded-lg bg-zinc-100">
                <Icon className="size-5 text-zinc-600" />
              </div>
            </CardHeader>
            <CardContent>
              <p className="text-xs text-muted-foreground">{description}</p>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
