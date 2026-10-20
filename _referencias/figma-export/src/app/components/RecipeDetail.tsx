import { useState } from 'react';
import { useParams, useNavigate } from 'react-router';
import { ArrowLeft, Clock, Users, Flame, ShoppingCart, Check } from 'lucide-react';
import { recipes } from '../data/recipes';

export function RecipeDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [completedSteps, setCompletedSteps] = useState<number[]>([]);
  const [activeTab, setActiveTab] = useState<'ingredientes' | 'pasos'>('ingredientes');

  const recipe = recipes.find(r => r.id === id);

  if (!recipe) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <p className="text-gray-600 mb-4">Receta no encontrada</p>
          <button
            onClick={() => navigate('/home')}
            className="px-6 py-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600"
          >
            Volver al inicio
          </button>
        </div>
      </div>
    );
  }

  const toggleStep = (index: number) => {
    setCompletedSteps(prev =>
      prev.includes(index) ? prev.filter(i => i !== index) : [...prev, index]
    );
  };

  return (
    <div className="min-h-screen pb-24">
      <div className="bg-gradient-to-br from-orange-200 to-amber-200 relative">
        <button
          onClick={() => navigate('/home')}
          className="absolute top-6 left-6 w-10 h-10 bg-white rounded-full flex items-center justify-center shadow-lg z-10"
        >
          <ArrowLeft size={20} className="text-gray-800" />
        </button>

        <div className="flex items-center justify-center h-64 text-8xl">
          {recipe.imagen}
        </div>
      </div>

      <div className="max-w-lg mx-auto px-6">
        <div className="bg-white rounded-t-3xl -mt-6 relative shadow-xl p-6">
          <h1 className="text-2xl mb-2 text-gray-800">{recipe.nombre}</h1>
          <p className="text-gray-600 mb-4">{recipe.descripcion}</p>

          <div className="flex gap-4 mb-6">
            <div className="flex items-center gap-2 px-4 py-2 bg-orange-50 rounded-lg">
              <Clock size={18} className="text-orange-600" />
              <span className="text-sm text-gray-700">{recipe.tiempoPreparacion} min</span>
            </div>
            <div className="flex items-center gap-2 px-4 py-2 bg-blue-50 rounded-lg">
              <Users size={18} className="text-blue-600" />
              <span className="text-sm text-gray-700">{recipe.porciones} porciones</span>
            </div>
            <div className="flex items-center gap-2 px-4 py-2 bg-red-50 rounded-lg">
              <Flame size={18} className="text-red-600" />
              <span className="text-sm text-gray-700">{recipe.calorias} kcal</span>
            </div>
          </div>

          {recipe.alergenos.length > 0 && (
            <div className="mb-6 p-4 bg-red-50 border-l-4 border-red-500 rounded">
              <p className="text-sm font-medium text-red-800 mb-1">⚠️ Contiene alérgenos:</p>
              <p className="text-sm text-red-700">{recipe.alergenos.join(', ')}</p>
            </div>
          )}

          <div className="flex gap-2 mb-6 border-b border-gray-200">
            <button
              onClick={() => setActiveTab('ingredientes')}
              className={`flex-1 pb-3 font-medium transition-colors ${
                activeTab === 'ingredientes'
                  ? 'text-orange-600 border-b-2 border-orange-600'
                  : 'text-gray-500'
              }`}
            >
              Ingredientes
            </button>
            <button
              onClick={() => setActiveTab('pasos')}
              className={`flex-1 pb-3 font-medium transition-colors ${
                activeTab === 'pasos'
                  ? 'text-orange-600 border-b-2 border-orange-600'
                  : 'text-gray-500'
              }`}
            >
              Pasos
            </button>
          </div>

          {activeTab === 'ingredientes' && (
            <div>
              <div className="space-y-3 mb-6">
                {recipe.ingredientes.map((ingrediente, index) => (
                  <div
                    key={index}
                    className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                  >
                    <span className="text-gray-800">{ingrediente.nombre}</span>
                    <span className="text-sm text-gray-600 font-medium">{ingrediente.cantidad}</span>
                  </div>
                ))}
              </div>

              <button
                onClick={() => navigate('/tiendas')}
                className="w-full flex items-center justify-center gap-2 px-6 py-3 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-colors"
              >
                <ShoppingCart size={20} />
                Ver dónde comprar
              </button>
            </div>
          )}

          {activeTab === 'pasos' && (
            <div className="space-y-4">
              {recipe.pasos.map((paso, index) => (
                <div
                  key={index}
                  onClick={() => toggleStep(index)}
                  className={`p-4 rounded-lg border-2 cursor-pointer transition-all ${
                    completedSteps.includes(index)
                      ? 'border-green-500 bg-green-50'
                      : 'border-gray-200 hover:border-orange-300'
                  }`}
                >
                  <div className="flex gap-3">
                    <div
                      className={`w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0 ${
                        completedSteps.includes(index)
                          ? 'bg-green-500 text-white'
                          : 'bg-orange-500 text-white'
                      }`}
                    >
                      {completedSteps.includes(index) ? <Check size={16} /> : index + 1}
                    </div>
                    <p className={`text-gray-800 ${completedSteps.includes(index) ? 'line-through' : ''}`}>
                      {paso}
                    </p>
                  </div>
                </div>
              ))}

              {completedSteps.length === recipe.pasos.length && (
                <div className="text-center p-6 bg-green-50 rounded-xl border-2 border-green-500">
                  <div className="text-5xl mb-2">🎉</div>
                  <h3 className="text-xl text-green-800 mb-1">¡Felicitaciones!</h3>
                  <p className="text-green-700">Has completado la receta</p>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
