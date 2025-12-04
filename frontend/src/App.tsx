import './App.css'
import AppRoutes from './app/routes/index.tsx'
import MainLayout from './shared/components/layout/MainLayout.tsx'

function App() {
  return (
    <div>
      <MainLayout>
        <AppRoutes />
      </MainLayout>
    </div>
  );
}

export default App;