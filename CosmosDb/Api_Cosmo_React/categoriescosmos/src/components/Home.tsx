import { useNavigate } from "react-router-dom";

function Home() {
  const navigate = useNavigate();

  return (
    <div className="container mt-5">
      <div className="text-center">
        <h1 className="display-4 fw-bold">Bienvenido a CategoriesApp</h1>
        <p className="lead text-muted mt-3">Gestiona tus categorías de forma sencilla</p>
        <button className="btn btn-primary btn-lg mt-4" onClick={() => navigate("/categories")}>
          Ver Categorías
        </button>
      </div>
    </div>
  );
}

export default Home;