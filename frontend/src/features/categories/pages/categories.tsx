import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Tags, Trash2, Plus } from 'lucide-react';
import { useCategories, useCreateCategory, useDeleteCategory } from '../hooks/use-categories';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';

const categorySchema = z.object({
  name: z.string().min(1, 'Name is required'),
  icon: z.string().optional(),
  color: z.string().optional(),
});

type CategoryForm = z.infer<typeof categorySchema>;

export function CategoriesPage() {
  const { data: categories, isLoading } = useCategories();
  const createMutation = useCreateCategory();
  const deleteMutation = useDeleteCategory();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CategoryForm>({
    resolver: zodResolver(categorySchema),
    defaultValues: { name: '', icon: '', color: '#6366f1' },
  });

  const onSubmit = (data: CategoryForm) => {
    createMutation.mutate(data, { onSuccess: () => reset() });
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Categories</h1>
        <p className="text-muted-foreground">
          Organize your transactions with custom categories.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Create Category</CardTitle>
          <CardDescription>Add a new transaction category.</CardDescription>
        </CardHeader>
        <CardContent>
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="flex flex-wrap items-end gap-3"
          >
            <div className="flex-1 space-y-2">
              <Label htmlFor="name">Name</Label>
              <Input
                id="name"
                placeholder="e.g. Groceries"
                {...register('name')}
                aria-invalid={!!errors.name}
              />
              {errors.name && (
                <p className="text-xs text-destructive">{errors.name.message}</p>
              )}
            </div>

            <div className="w-28 space-y-2">
              <Label htmlFor="icon">Icon</Label>
              <Input
                id="icon"
                placeholder="e.g. cart"
                {...register('icon')}
              />
            </div>

            <div className="w-20 space-y-2">
              <Label htmlFor="color">Color</Label>
              <Input
                id="color"
                type="color"
                className="h-8 cursor-pointer p-1"
                {...register('color')}
              />
            </div>

            <Button type="submit" disabled={createMutation.isPending}>
              <Plus className="size-4" />
              Add
            </Button>
          </form>
        </CardContent>
      </Card>

      {isLoading && (
        <p className="text-sm text-muted-foreground">Loading categories...</p>
      )}

      {categories && categories.length === 0 && (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12">
            <Tags className="mb-3 size-10 text-muted-foreground" />
            <p className="text-sm font-medium">No categories yet</p>
            <p className="text-xs text-muted-foreground">
              Create your first category above.
            </p>
          </CardContent>
        </Card>
      )}

      {categories && categories.length > 0 && (
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
          {categories.map((category) => (
            <Card key={category.id}>
              <CardContent className="flex items-center justify-between py-3">
                <div className="flex items-center gap-3">
                  <div
                    className="flex size-8 items-center justify-center rounded-lg"
                    style={{ backgroundColor: category.color ?? '#6366f1' }}
                  >
                    <Tags className="size-4 text-white" />
                  </div>
                  <span className="text-sm font-medium">{category.name}</span>
                </div>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => deleteMutation.mutate(category.id)}
                  disabled={deleteMutation.isPending}
                >
                  <Trash2 className="size-4 text-muted-foreground" />
                </Button>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
