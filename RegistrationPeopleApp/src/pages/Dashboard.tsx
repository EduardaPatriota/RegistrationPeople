import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, LogOut, Lock, Search } from 'lucide-react';
import Button from '@/components/Button';
import PersonForm from '@/components/PersonForm';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { apiGet, apiPost, apiPut, apiDelete } from '../utils/api';

interface Person {
  id: string;
  name: string;
  email: string;
  cpf: string;
  gender: string;
  birthDate: string;
  birthplace: string;
  nationality: string;
  address: string;
  password?: string;
}

const Dashboard = () => {
  const [people, setPeople] = useState<Person[]>([]);
  const [editingPerson, setEditingPerson] = useState<Person | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [search, setSearch] = useState('');

  const fetchPeople = async () => {
    try {
      const response = await apiGet('/v1/Person');
      setPeople(response.data);
    } catch (error) {
      console.error('Erro ao buscar pessoas:', error);
    }
  };

  useEffect(() => {
    fetchPeople();
  }, []);

  function getPersonEndpoint(personData: { apiVersion?: string }, id?: string) {
    const version = personData.apiVersion === 'v2' ? 'v2' : 'v1';
    let endpoint = `/${version}/Person`;
    if (id) endpoint += `/${id}`;
    return endpoint;
  }

  const handleAddPerson = async (personData: Omit<Person, 'id'> & { apiVersion?: string }) => {
    try {
      const { apiVersion, ...dataToSend } = personData;
      const response = await apiPost(getPersonEndpoint(personData), dataToSend);
      setPeople([...people, response.data]);
      setIsDialogOpen(false);
    } catch (error) {
      console.error('Erro ao adicionar pessoa:', error);
    }
  };

  const handleEditPerson = async (personData: Omit<Person, 'id'> & { apiVersion?: string }) => {
    if (editingPerson) {
      try {
        const { apiVersion, ...dataToSend } = personData;
        const response = await apiPut(getPersonEndpoint(personData, editingPerson.id), dataToSend);

          await fetchPeople();
          setEditingPerson(null);
          setIsDialogOpen(false);
          console.warn('Nenhum dado retornado após a edição. Atualizando lista de pessoas.');
        }
       catch (error) {
        console.error('Erro ao editar pessoa:', error);
      }
    }
  };

  const handleDeletePerson = async (id: string) => {
    try {
      await apiDelete(`/v1/Person/${id}`);
      await fetchPeople();
    } catch (error) {
      console.error('Erro ao deletar pessoa:', error);
    }
  };

  const openEditDialog = async (person: Person) => {
    try {
      const response = await apiGet(`/v1/Person/${person.id}`);
      console.log('Dados recebidos para edição:', response.data); 
      setEditingPerson(response.data);
      setIsDialogOpen(true);
    } catch (error) {
      console.error('Erro ao buscar dados da pessoa para edição:', error);
    }
  };

  const openAddDialog = () => {
    setEditingPerson(null);
    setIsDialogOpen(true);
  };

  const handleLogout = () => {
    window.location.href = '/';
  };

  
  const filteredPeople = people.filter(person =>
  (person.name?.toLowerCase() || '').includes(search.toLowerCase()) ||
  (person.email?.toLowerCase() || '').includes(search.toLowerCase()) ||
  (person.cpf || '').includes(search)
);

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-purple-50 p-6">
      <div className="max-w-6xl mx-auto">
        <header className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-800">Sistema de Registro de Pessoas</h1>
            <p className="text-gray-600 mt-2">Gerencie o cadastro de pessoas do sistema</p>
          </div>
          <Button variant="outline" onClick={handleLogout}>
            <LogOut className="w-4 h-4 mr-2" />
            Sair
          </Button>
        </header>

        <div className="bg-white/90 backdrop-blur-sm rounded-2xl shadow-xl border border-white/20 p-6">
          <div className="flex justify-between items-center mb-6">
            <h2 className="text-2xl font-semibold text-gray-800">Pessoas Cadastradas</h2>
            <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
              <DialogTrigger asChild>
                <Button onClick={openAddDialog}>
                  <Plus className="w-4 h-4 mr-2" />
                  Nova Pessoa
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-md">
                <DialogHeader>
                  <DialogTitle>
                    {editingPerson ? 'Editar Pessoa' : 'Nova Pessoa'}
                  </DialogTitle>
                </DialogHeader>
                <PersonForm
                  person={editingPerson}
                  onSubmit={editingPerson ? handleEditPerson : handleAddPerson}
                  onCancel={() => setIsDialogOpen(false)}
                />
              </DialogContent>
            </Dialog>
          </div>

          <div className="mb-4 flex items-center gap-2">
            <Search className="w-4 h-4 text-gray-400" />
            <input
              type="text"
              placeholder="Buscar por nome, email ou CPF"
              value={search}
              onChange={e => setSearch(e.target.value)}
              className="border rounded px-3 py-2 w-full max-w-xs text-sm"
            />
          </div>

          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Nome</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>CPF</TableHead>
                  <TableHead>Endereço</TableHead>
                  <TableHead className="text-right">Ações</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredPeople.map((person, idx) => (
                  <TableRow key={person.id || person.email || idx}>
                    <TableCell className="font-medium flex items-center gap-1">
                      {person.name}
                      {person.password && (
                        <span title="Usuário com acesso">
                          <Lock className="w-4 h-4 text-gray-500 ml-1" aria-label="Usuário com acesso" />
                        </span>
                      )}
                    </TableCell>
                    <TableCell>{person.email}</TableCell>
                    <TableCell>{person.cpf}</TableCell>
                    <TableCell>
                      {person.address ? person.address : '-'}
                    </TableCell>
                    <TableCell className="text-right">
                      <div className="flex gap-2 justify-end">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => openEditDialog(person)}
                        >
                          <Edit className="w-4 h-4" />
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleDeletePerson(person.id)}
                          className="text-red-600 hover:text-red-700 hover:border-red-300"
                        >
                          <Trash2 className="w-4 h-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>

          {filteredPeople.length === 0 && (
            <div className="text-center py-8">
              <p className="text-gray-500">Nenhuma pessoa cadastrada ainda.</p>
              <p className="text-gray-400 text-sm mt-2">Clique em "Nova Pessoa" para começar.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Dashboard;