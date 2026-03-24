import { Route, Routes } from "react-router-dom";
import CategoryForm from "./components/CategoryForm";
import CategoryList from "./components/CategoryList";
import Home from "./components/Home";
import Navbar from "./components/Navbar";

function AppRouter() {
  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/categories" element={<CategoryList />} />
        <Route path="/create" element={<CategoryForm />} />
        <Route path="/edit/:id" element={<CategoryForm />} />
      </Routes>
    </>
  );
}

export default AppRouter;