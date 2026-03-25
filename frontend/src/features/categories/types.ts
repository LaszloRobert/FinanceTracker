export interface Category {
  id: string;
  name: string;
  icon: string | null;
  color: string | null;
  parentCategoryId: string | null;
  isDefault: boolean;
}
