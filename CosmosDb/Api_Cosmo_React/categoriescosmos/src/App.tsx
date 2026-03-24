import { BrowserRouter } from "react-router-dom";
import AppRouter from "./AppRouter";
import { CategoryProvider } from "./context/CategoryContext";

function App() {
  return (
    <BrowserRouter>
      <CategoryProvider>
        <AppRouter />
      </CategoryProvider>
    </BrowserRouter>
  );
}

export default App;

