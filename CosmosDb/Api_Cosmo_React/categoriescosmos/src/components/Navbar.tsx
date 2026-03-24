import { useNavigate } from "react-router-dom";

function Navbar() {
  const navigate = useNavigate();

  return (
    <nav className="navbar navbar-expand-sm navbar-dark bg-dark">
      <div className="container">
        <span className="navbar-brand" style={{ cursor: "pointer" }} onClick={() => navigate("/")}>CategoriesApp</span>
        <div className="collapse navbar-collapse">
          <ul className="navbar-nav">
            <li className="nav-item">
              <span className="nav-link" style={{ cursor: "pointer" }} onClick={() => navigate("/")}>Home</span>
            </li>
            <li className="nav-item">
              <span className="nav-link" style={{ cursor: "pointer" }} onClick={() => navigate("/categories")}>Categorías</span>
            </li>
            <li className="nav-item">
              <span className="nav-link" style={{ cursor: "pointer" }} onClick={() => navigate("/create")}>Nueva Categoría</span>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;