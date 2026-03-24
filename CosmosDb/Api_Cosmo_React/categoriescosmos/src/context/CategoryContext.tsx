import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type { Category } from "../model/Category";
import categoryService from "../services/categoryService";

interface CategoryContextType {
  categories: Category[];
  loadCategories: () => void;
  handleDelete: (id: string) => Promise<void>;
  handleSubmit: (form: Category, isEditing: boolean, id?: string, onSuccess?: () => void) => Promise<void>;
}

interface CategoryContextType {
  categories: Category[];
  alert: { message: string; type: "success" | "danger" } | null;
  loadCategories: () => void;
  handleDelete: (id: string) => Promise<void>;
  handleSubmit: (form: Category, isEditing: boolean, id?: string, onSuccess?: () => void) => Promise<void>;
}

const CategoryContext = createContext<CategoryContextType | undefined>(undefined);

export function CategoryProvider({ children }: { children: ReactNode }) {
  const [categories, setCategories] = useState<Category[]>([]);
  const [alert, setAlert] = useState<{ message: string; type: "success" | "danger" } | null>(null);

  const showAlert = (message: string, type: "success" | "danger") => {
    setAlert({ message, type });
    setTimeout(() => setAlert(null), 5000);
  };

  const loadCategories = async () => {
    try {
        const response = await categoryService.getAll();
        setCategories(response.data);
    } catch {
        showAlert("Error al conectar con el servidor", "danger");
    }
};

  useEffect(() => {
    loadCategories();
  }, []);

  const handleDelete = async (id: string) => {
    try {
      await categoryService.remove(id);
      await loadCategories();
      showAlert("Categoría eliminada correctamente", "success");
    } catch {
      showAlert("Error al eliminar la categoría", "danger");
    }
  };

  const handleSubmit = async (form: Category, isEditing: boolean, id?: string, onSuccess?: () => void) => {
    try {
      if (isEditing && id) {
        await categoryService.update(id, form);
        showAlert("Categoría actualizada correctamente", "success");
      } else {
        const response = await categoryService.getAll();
        const maxId = response.data.reduce((max, cat) => {
          const num = parseInt(cat.categoryID || "0");
          return num > max ? num : max;
        }, 0);
        form.categoryID = (maxId + 1).toString();
        await categoryService.create(form);
        showAlert("Categoría creada correctamente", "success");
      }
      await loadCategories();
      onSuccess?.();
    } catch (error) {
      console.log("Error capturado:", error);
      showAlert("Error al conectar con el servidor", "danger");
    }
  };

  return (
    <CategoryContext.Provider value={{ categories, alert, loadCategories, handleDelete, handleSubmit }}>
      {children}
    </CategoryContext.Provider>
  );
}

export function useCategoryContext() {
  const context = useContext(CategoryContext);
  if (!context) throw new Error("useCategoryContext debe usarse dentro de CategoryProvider");
  return context;
}