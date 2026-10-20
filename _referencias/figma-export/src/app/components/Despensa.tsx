import { useState, useMemo, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router';
import { Plus, X, ShoppingCart, ChefHat, Search, CheckCircle, AlertCircle, ArrowRight, Refrigerator, ChevronDown } from 'lucide-react';
import { recipes } from '../data/recipes';
import { useUser } from '../context/UserContext';

const SUGERENCIAS_COMUNES = [
  'Huevos', 'Leche', 'Mantequilla', 'Aceite de oliva', 'Ajo', 'Cebolla',
  'Tomate', 'Papa', 'Zanahoria', 'Sal', 'Pimienta', 'Harina', 'Arroz',
  'Pasta', 'Lenteja', 'Pollo', 'Queso', 'Limón', 'Espinaca', 'Pimiento',
];

interface RecetaConAnalisis {
  id: string;
  nombre: string;
  imagen: string;
  tiempoPreparacion: number;
  dificultad: string;
  ingredientesDisponibles: string[];
  ingredientesFaltantes: { nombre: string; cantidad: string }[];
  porcentaje: number;
}

function normalizar(texto: string) {
  return texto.toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '');
}

function tieneCoincidencia(disponible: string, requerido: string) {
  const d = normalizar(disponible);
  const r = normalizar(requerido);
  return r.includes(d) || d.includes(r);
}

const STORAGE_KEY = 'cocina-facil-despensa';

function cargarDespensa(): string[] {
  try {
    const guardado = localStorage.getItem(STORAGE_KEY);
    return guardado ? JSON.parse(guardado) : [];
  } catch {
    return [];
  }
}

export function Despensa() {
  const navigate = useNavigate();
  const { preferences } = useUser();
  const [ingredientes, setIngredientes] = useState<string[]>(cargarDespensa);
  const [inputValor, setInputValor] = useState('');
  const [dropdownAbierto, setDropdownAbierto] = useState(false);
  const [vistaActiva, setVistaActiva] = useState<'completas' | 'parciales'>('completas');
  const comboboxRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function cerrarAlClickAfuera(e: MouseEvent) {
      if (comboboxRef.current && !comboboxRef.current.contains(e.target as Node)) {
        setDropdownAbierto(false);
      }
    }
    document.addEventListener('mousedown', cerrarAlClickAfuera);
    return () => document.removeEventListener('mousedown', cerrarAlClickAfuera);
  }, []);

  const actualizarIngredientes = (nuevos: string[]) => {
    setIngredientes(nuevos);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(nuevos));
  };

  const agregarIngrediente = (nombre: string) => {
    const limpio = nombre.trim();
    if (!limpio) return;
    if (ingredientes.some(i => normalizar(i) === normalizar(limpio))) return;
    actualizarIngredientes([...ingredientes, limpio]);
    setInputValor('');
    setDropdownAbierto(false);
  };

  const quitarIngrediente = (nombre: string) => {
    actualizarIngredientes(ingredientes.filter(i => i !== nombre));
  };

  const opcionesDropdown = SUGERENCIAS_COMUNES.filter(s => {
    const noAgregado = !ingredientes.some(i => normalizar(i) === normalizar(s));
    if (inputValor.length === 0) return noAgregado;
    return noAgregado && normalizar(s).includes(normalizar(inputValor));
  });

  const mostrarOpcionCustom =
    inputValor.trim().length > 0 &&
    !SUGERENCIAS_COMUNES.some(s => normalizar(s) === normalizar(inputValor.trim())) &&
    !ingredientes.some(i => normalizar(i) === normalizar(inputValor.trim()));

  const analisisRecetas = useMemo((): RecetaConAnalisis[] => {
    if (ingredientes.length === 0) return [];

    return recipes.map(receta => {
      const disponibles: string[] = [];
      const faltantes: { nombre: string; cantidad: string }[] = [];

      receta.ingredientes.forEach(ing => {
        const encontrado = ingredientes.some(disp => tieneCoincidencia(disp, ing.nombre));
        if (encontrado) {
          disponibles.push(ing.nombre);
        } else {
          faltantes.push(ing);
        }
      });

      const porcentaje = Math.round((disponibles.length / receta.ingredientes.length) * 100);

      return {
        id: receta.id,
        nombre: receta.nombre,
        imagen: receta.imagen,
        tiempoPreparacion: receta.tiempoPreparacion,
        dificultad: receta.dificultad,
        ingredientesDisponibles: disponibles,
        ingredientesFaltantes: faltantes,
        porcentaje,
      };
    });
  }, [ingredientes]);

  const recetasCompletas = analisisRecetas.filter(r => r.porcentaje === 100);
  const recetasParciales = analisisRecetas
    .filter(r => r.porcentaje >= 25 && r.porcentaje < 100)
    .sort((a, b) => b.porcentaje - a.porcentaje);

  return (
    <div className="min-h-screen pb-28">
      {/* Header */}
      <div className="bg-gradient-to-r from-emerald-500 to-teal-500 text-white p-6 pb-8">
        <div className="max-w-lg mx-auto">
          <div className="flex items-center gap-3 mb-1">
            <Refrigerator size={28} />
            <h1 className="text-2xl">Mi Despensa</h1>
          </div>
          <p className="text-emerald-100">
            Ingresá los ingredientes que tenés en casa y te sugerimos recetas
          </p>
        </div>
      </div>

      <div className="max-w-lg mx-auto px-4 -mt-4 space-y-4">
        {/* Combobox de ingredientes */}
        <div className="bg-white rounded-xl shadow-lg p-4">
          <h2 className="font-medium text-gray-700 mb-3 flex items-center gap-2">
            <Plus size={18} className="text-emerald-500" />
            Agregar ingrediente
          </h2>
          <div className="flex gap-2">
            <div className="relative flex-1" ref={comboboxRef}>
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none" size={18} />
              <input
                type="text"
                value={inputValor}
                onChange={e => { setInputValor(e.target.value); setDropdownAbierto(true); }}
                onFocus={() => setDropdownAbierto(true)}
                onKeyDown={e => {
                  if (e.key === 'Enter') { agregarIngrediente(inputValor); }
                  if (e.key === 'Escape') { setDropdownAbierto(false); }
                }}
                placeholder="Escribí o elegí de la lista..."
                className="w-full pl-10 pr-8 py-2.5 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-300 text-gray-800"
              />
              <button
                onMouseDown={e => { e.preventDefault(); setDropdownAbierto(v => !v); }}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-emerald-600"
              >
                <ChevronDown size={16} className={`transition-transform ${dropdownAbierto ? 'rotate-180' : ''}`} />
              </button>

              {/* Dropdown */}
              {dropdownAbierto && (
                <div className="absolute z-20 left-0 right-0 top-full mt-1 bg-white border border-gray-200 rounded-xl shadow-xl max-h-56 overflow-y-auto">
                  {mostrarOpcionCustom && (
                    <button
                      onMouseDown={e => { e.preventDefault(); agregarIngrediente(inputValor); }}
                      className="w-full text-left px-4 py-2.5 text-sm hover:bg-emerald-50 text-emerald-700 border-b border-gray-100 flex items-center gap-2"
                    >
                      <Plus size={14} />
                      Agregar "{inputValor.trim()}"
                    </button>
                  )}
                  {opcionesDropdown.length > 0 ? (
                    opcionesDropdown.map(s => (
                      <button
                        key={s}
                        onMouseDown={e => { e.preventDefault(); agregarIngrediente(s); }}
                        className="w-full text-left px-4 py-2.5 text-sm hover:bg-emerald-50 text-gray-700 flex items-center gap-2"
                      >
                        <span className="w-5 h-5 rounded-full bg-emerald-100 text-emerald-600 flex items-center justify-center text-xs flex-shrink-0">+</span>
                        {s}
                      </button>
                    ))
                  ) : (
                    !mostrarOpcionCustom && (
                      <p className="px-4 py-3 text-sm text-gray-400">No hay más sugerencias disponibles</p>
                    )
                  )}
                </div>
              )}
            </div>
            <button
              onClick={() => agregarIngrediente(inputValor)}
              className="px-4 py-2.5 bg-emerald-500 text-white rounded-lg hover:bg-emerald-600 transition-colors flex-shrink-0"
            >
              <Plus size={20} />
            </button>
          </div>
        </div>

        {/* Lista de ingredientes actuales */}
        {ingredientes.length > 0 && (
          <div className="bg-white rounded-xl shadow-md p-4">
            <div className="flex items-center justify-between mb-3">
              <h2 className="font-medium text-gray-700">
                Tengo en casa ({ingredientes.length})
              </h2>
              <button
                onClick={() => actualizarIngredientes([])}
                className="text-xs text-red-400 hover:text-red-600"
              >
                Limpiar todo
              </button>
            </div>
            <div className="flex flex-wrap gap-2">
              {ingredientes.map(ing => (
                <span
                  key={ing}
                  className="flex items-center gap-1.5 px-3 py-1.5 bg-emerald-100 text-emerald-800 rounded-full text-sm"
                >
                  {ing}
                  <button onClick={() => quitarIngrediente(ing)} className="hover:text-red-500">
                    <X size={14} />
                  </button>
                </span>
              ))}
            </div>
          </div>
        )}

        {/* Resultados */}
        {ingredientes.length > 0 && (
          <>
            {/* Tabs */}
            <div className="flex bg-white rounded-xl shadow-md overflow-hidden">
              <button
                onClick={() => setVistaActiva('completas')}
                className={`flex-1 py-3 text-sm font-medium flex items-center justify-center gap-2 transition-colors ${
                  vistaActiva === 'completas'
                    ? 'bg-emerald-500 text-white'
                    : 'text-gray-600 hover:bg-gray-50'
                }`}
              >
                <CheckCircle size={16} />
                Podés cocinar ({recetasCompletas.length})
              </button>
              <button
                onClick={() => setVistaActiva('parciales')}
                className={`flex-1 py-3 text-sm font-medium flex items-center justify-center gap-2 transition-colors ${
                  vistaActiva === 'parciales'
                    ? 'bg-amber-500 text-white'
                    : 'text-gray-600 hover:bg-gray-50'
                }`}
              >
                <AlertCircle size={16} />
                Casi listas ({recetasParciales.length})
              </button>
            </div>

            {/* Recetas completas */}
            {vistaActiva === 'completas' && (
              <div className="space-y-3">
                {recetasCompletas.length === 0 ? (
                  <div className="bg-white rounded-xl shadow-md p-6 text-center">
                    <ChefHat size={40} className="mx-auto text-gray-300 mb-3" />
                    <p className="text-gray-500">Ninguna receta se puede hacer con estos ingredientes solos.</p>
                    <p className="text-sm text-gray-400 mt-1">Revisá las recetas "Casi listas" para ver qué te falta comprar.</p>
                  </div>
                ) : (
                  recetasCompletas.map(receta => (
                    <div
                      key={receta.id}
                      onClick={() => navigate(`/receta/${receta.id}`)}
                      className="bg-white rounded-xl shadow-md overflow-hidden cursor-pointer hover:shadow-xl transition-shadow border-l-4 border-emerald-500"
                    >
                      <div className="flex items-center p-4 gap-4">
                        <div className="w-16 h-16 bg-gradient-to-br from-emerald-100 to-teal-100 rounded-xl flex items-center justify-center text-4xl flex-shrink-0">
                          {receta.imagen}
                        </div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2 mb-1">
                            <CheckCircle size={14} className="text-emerald-500 flex-shrink-0" />
                            <h3 className="font-medium text-gray-800 truncate">{receta.nombre}</h3>
                          </div>
                          <p className="text-xs text-emerald-600 mb-2">Tenés todos los ingredientes</p>
                          <div className="flex items-center gap-3 text-xs text-gray-500">
                            <span>⏱ {receta.tiempoPreparacion} min</span>
                            <span className={`px-2 py-0.5 rounded-full ${
                              receta.dificultad === 'fácil' ? 'bg-green-100 text-green-700' :
                              receta.dificultad === 'media' ? 'bg-yellow-100 text-yellow-700' :
                              'bg-red-100 text-red-700'
                            }`}>{receta.dificultad}</span>
                          </div>
                        </div>
                        <ArrowRight size={20} className="text-gray-400 flex-shrink-0" />
                      </div>
                    </div>
                  ))
                )}
              </div>
            )}

            {/* Recetas parciales */}
            {vistaActiva === 'parciales' && (
              <div className="space-y-3">
                {recetasParciales.length === 0 ? (
                  <div className="bg-white rounded-xl shadow-md p-6 text-center">
                    <ShoppingCart size={40} className="mx-auto text-gray-300 mb-3" />
                    <p className="text-gray-500">No hay recetas con al menos el 50% de ingredientes disponibles.</p>
                    <p className="text-sm text-gray-400 mt-1">Agregá más ingredientes para ver más sugerencias.</p>
                  </div>
                ) : (
                  recetasParciales.map(receta => (
                    <div key={receta.id} className="bg-white rounded-xl shadow-md overflow-hidden border-l-4 border-amber-400">
                      <div
                        className="flex items-center p-4 gap-4 cursor-pointer hover:bg-gray-50 transition-colors"
                        onClick={() => navigate(`/receta/${receta.id}`)}
                      >
                        <div className="w-16 h-16 bg-gradient-to-br from-amber-100 to-orange-100 rounded-xl flex items-center justify-center text-4xl flex-shrink-0">
                          {receta.imagen}
                        </div>
                        <div className="flex-1 min-w-0">
                          <h3 className="font-medium text-gray-800 truncate mb-1">{receta.nombre}</h3>
                          {/* Barra de progreso */}
                          <div className="flex items-center gap-2 mb-2">
                            <div className="flex-1 h-2 bg-gray-100 rounded-full overflow-hidden">
                              <div
                                className="h-full bg-gradient-to-r from-amber-400 to-orange-400 rounded-full transition-all"
                                style={{ width: `${receta.porcentaje}%` }}
                              />
                            </div>
                            <span className="text-xs text-amber-600 font-medium w-10 text-right">
                              {receta.porcentaje}%
                            </span>
                          </div>
                          <div className="flex items-center gap-3 text-xs text-gray-500">
                            <span>⏱ {receta.tiempoPreparacion} min</span>
                            <span className={`px-2 py-0.5 rounded-full ${
                              receta.dificultad === 'fácil' ? 'bg-green-100 text-green-700' :
                              receta.dificultad === 'media' ? 'bg-yellow-100 text-yellow-700' :
                              'bg-red-100 text-red-700'
                            }`}>{receta.dificultad}</span>
                          </div>
                        </div>
                        <ArrowRight size={20} className="text-gray-400 flex-shrink-0" />
                      </div>

                      {/* Ingredientes faltantes */}
                      <div className="px-4 pb-4 pt-0">
                        <div className="bg-amber-50 rounded-lg p-3">
                          <div className="flex items-center justify-between mb-2">
                            <p className="text-xs font-medium text-amber-800 flex items-center gap-1">
                              <ShoppingCart size={13} />
                              Necesitás comprar ({receta.ingredientesFaltantes.length}):
                            </p>
                            <button
                              onClick={() => navigate('/tiendas')}
                              className="text-xs text-amber-600 hover:text-amber-800 flex items-center gap-1 font-medium"
                            >
                              Ver tiendas <ArrowRight size={12} />
                            </button>
                          </div>
                          <div className="flex flex-wrap gap-1.5">
                            {receta.ingredientesFaltantes.map(ing => (
                              <span
                                key={ing.nombre}
                                className="px-2 py-1 bg-white border border-amber-200 text-amber-800 rounded-md text-xs"
                              >
                                {ing.nombre} <span className="text-amber-500">({ing.cantidad})</span>
                              </span>
                            ))}
                          </div>
                        </div>
                      </div>
                    </div>
                  ))
                )}
              </div>
            )}
          </>
        )}

        {/* Estado vacío inicial */}
        {ingredientes.length === 0 && (
          <div className="bg-white rounded-xl shadow-md p-8 text-center">
            <div className="text-6xl mb-4">🥦</div>
            <h3 className="text-gray-700 mb-2">¿Qué tenés en casa?</h3>
            <p className="text-sm text-gray-400">
              Ingresá los ingredientes disponibles y te mostraremos qué podés cocinar ahora mismo.
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
