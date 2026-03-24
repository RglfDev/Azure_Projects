
import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useCategoryContext } from "../context/CategoryContext";
import categoryService from "../services/categoryService";
import type { Category } from "../model/Category";


const emptyForm: Category = {
  categoryID: "",
  categoryName: "",
  description: "",
  picture: ""
};

const emptyErrors = {
  categoryName: "",
  description: "",
  picture: ""
};

function CategoryForm() {
  const { id } = useParams<{ id: string }>();
  const { handleSubmit, alert } = useCategoryContext();
  const navigate = useNavigate();
  const [form, setForm] = useState<Category>(emptyForm);
  const [errors, setErrors] = useState(emptyErrors);
  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      categoryService.getById(id!).then((response) => setForm(response.data));
    }
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setErrors({ ...errors, [e.target.name]: "" });
  };

  const validate = (): boolean => {
    const newErrors = { ...emptyErrors };
    let isValid = true;
    if (!form.categoryName.trim()) { newErrors.categoryName = "El nombre es obligatorio"; isValid = false; }
    if (!form.description.trim()) { newErrors.description = "La descripción es obligatoria"; isValid = false; }
    if (!form.picture.trim()) { newErrors.picture = "La imagen es obligatoria"; isValid = false; }
    setErrors(newErrors);
    return isValid;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    await handleSubmit(form, isEditing, id, () => navigate("/categories"));
  };

  return (
    <div className="d-flex flex-column justify-content-center align-items-center" style={{ minHeight: "80vh" }}>
      <div className="card shadow" style={{ width: "400px" }}>
        <div className="card-body p-4">
          <h3 className="card-title text-center mb-4">{isEditing ? "Editar Categoría" : "Nueva Categoría"}</h3>
          <form onSubmit={onSubmit}>
            <div className="mb-3">
              <label className="form-label">Nombre</label>
              <input
                className={`form-control ${errors.categoryName ? "is-invalid" : ""}`}
                name="categoryName"
                placeholder="Nombre"
                value={form.categoryName}
                onChange={handleChange}
              />
              {errors.categoryName && <div className="invalid-feedback">{errors.categoryName}</div>}
            </div>
            <div className="mb-3">
              <label className="form-label">Descripción</label>
              <input
                className={`form-control ${errors.description ? "is-invalid" : ""}`}
                name="description"
                placeholder="Descripción"
                value={form.description}
                onChange={handleChange}
              />
              {errors.description && <div className="invalid-feedback">{errors.description}</div>}
            </div>
            <div className="mb-3">
              <label className="form-label">Picture</label>
              <input
                className={`form-control ${errors.picture ? "is-invalid" : ""}`}
                name="picture"
                placeholder="Picture"
                value={form.picture}
                onChange={handleChange}
              />
              {errors.picture && <div className="invalid-feedback">{errors.picture}</div>}
            </div>
            <div className="d-flex justify-content-end gap-2 mt-4">
              <button type="button" className="btn btn-secondary" onClick={() => navigate("/categories")}>Cancelar</button>
              <button type="submit" className="btn btn-primary">{isEditing ? "Actualizar" : "Crear"}</button>
            </div>
          </form>
        </div>
      </div>

      {alert && (
        <div className={`alert alert-${alert.type} mt-3`} style={{ width: "400px" }} role="alert">
          {alert.message}
        </div>
      )}
    </div>
  );
}

export default CategoryForm;