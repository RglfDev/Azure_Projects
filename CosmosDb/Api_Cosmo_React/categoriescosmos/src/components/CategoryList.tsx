import { useNavigate } from "react-router-dom";
import { useCategoryContext } from "../context/CategoryContext";

function CategoryList() {
  const { categories, handleDelete, alert } = useCategoryContext();
  const navigate = useNavigate();

  return (
    <div className="container mt-4">
      <h1 className="mb-4">Categorías</h1>

      {alert && (
        <div
          className={`alert alert-${alert.type} alert-dismissible`}
          role="alert"
        >
          {alert.message}
        </div>
      )}

      <button
        className="btn btn-primary mb-3"
        onClick={() => navigate("/create")}
      >
        Nueva Categoría
      </button>
      <table className="table table-striped table-bordered">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {categories.length === 0 ? (
            <tr>
              <td colSpan={4} className="text-center text-muted py-4">
                No hay categorías disponibles
              </td>
            </tr>
          ) : (
            categories.map((cat) => (
              <tr key={cat.categoryID}>
                <td>{cat.categoryID}</td>
                <td>{cat.categoryName}</td>
                <td>{cat.description}</td>
                <td>
                  <button
                    className="btn btn-warning btn-sm me-2"
                    onClick={() => navigate(`/edit/${cat.categoryID}`)}
                  >
                    Editar
                  </button>
                  <button
                    className="btn btn-danger btn-sm"
                    onClick={() => handleDelete(cat.categoryID!)}
                  >
                    Eliminar
                  </button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}

export default CategoryList;
