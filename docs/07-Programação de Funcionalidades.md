# Programação de Funcionalidades (INCLUIR A PROGRAMAÇAÕ DE FUNCIONALIDADE EM PROFUNDIDADE)

<span style="color:red">Pré-requisitos: <a href="2-Especificação do Projeto.md"> Especificação do Projeto</a></span>, <a href="3-Projeto de Interface.md"> Projeto de Interface</a>, <a href="4-Metodologia.md"> Metodologia</a>, <a href="3-Projeto de Interface.md"> Projeto de Interface</a>, <a href="5-Arquitetura da Solução.md"> Arquitetura da Solução</a>

Nesta seção, a implementação do sistema descrita por meio dos requisitos funcionais e/ou não funcionais. Nesta seção, é essencial relacionar os requisitos atendidos com os artefatos criados (código fonte) e com o(s) responsável(is) pelo desenvolvimento de cada artefato a cada etapa. Nesta seção também deverão ser apresentadas, se necessário, as instruções para acesso e verificação da **implementação que deve estar funcional no ambiente de hospedagem, OBRIGATORIAMENTE, a partir da Etapa 03**.

**O que DEVE ser utilizado para o desenvolvimento da aplicação:**
- Microsoft Visual Studio (IDE de Codificação)
- HTML e CSS (frontend)
- Javascript (frontend)
- C# (backend)
- MySQL ou SQLServer(Base de Dados)
- Bootstrap (template responsivo para frontend)
- Github (documentação e controle de versão)

**O que NÃO PODE ser utilizado:**
- Template React (e qualquer outro template - exceto o Bootstrap)
- Qualquer outra liguagem de programação diferente de C#

A tabela a seguir é um exemplo de como ela deverá ser preenchida considerando os artefatos desenvolvidos.

|ID    | Descrição do Requisito  | Artefatos produzidos | Aluno(a) responsável |
|------|-----------------------------------------|----|----|
|RF-001| A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha.|AuthController.cs, UsuariosController.cs, Models\Usuario, Models\Enum, Models\AppDbContext, Views\Usuarios, ViewsModels, Services\EmailService.cs, Services\IEmailService.cs  | Paulo |
|RF-002| A aplicação deve exigir o aceite digital obrigatório do "Termo de Responsabilidade" (baseado na Lei 14.016/2020) no momento do cadastro do doador.|Models\TermoAceitacao; Auth\Login.cshtml; Auth\RecuperarSenha.cshtml; Auth\Registro.cshtml; Auth\ResetSenha.cshtml; Auth\Termo.cshtml; Shared\_Layout.cshtml | Paulo |
|RF-003| A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador.| |Gabriel  |
|RF-004| A aplicação deve permitir que o doador cadastre itens para doação com as informações dos produtos.| | Victor |
|RF-005| A aplicação deve exibir uma vitrine em tempo real das doações disponíveis, permitindo que o beneficário possa realizar filtros.| Produto.cs; Reserva.cs; VitrineCompleteViewModel.cs; VitrineDoacoesViewModel.cs; VitrineFiltroViewModel.cs  |Wadahan  |
|RF-006| A aplicação deve permitir que o beneficiário solicite e reserve uma doação, alterando o status do item no sistema para evitar que outra pessoa reserve o mesmo alimento| CarrinhoItem.cs; Lote.cs; Pedido.cs; ValorLookUp.cs; CarrinhoItemViewModel.cs; CarrinhoViewModel.cs; PedidoConfirmadoViewModel.cs; CarrinhoController.cs; LookupController.cs; ReservasController.cs; Carrinho/Confirmado.cshtml; Carrinho/Index.cshtml; Lookup/Index.cshtml; Reservas/MinhasReservas.cshtml | Wadahan/Victor |
|RF-007| A aplicação deve permitir que o doador valide a entrega da doação através de um código numérico ou QR Code apresentado pelo receptor no momento da retirada.| GerenciarReservaDoadorViewModel.cs; GerenciarReservasPagesViewModel.cs; Produtos/GerenciarReservas.cshtml | Wadahan/Victor |
|RF-008| A aplicação deve possuir um painel administrativo para moderação de conteúdo e gerenciamento de usuários.| |Deivid  |
|RF-009| A aplicação deve notificar os usuários sobre aprovação ou recusa de reservas, lembretes de retirada e doações expiradas.| |Gabriel  |
|RF-010| A aplicação deve gerar relatórios de impacto exibindo o volume total de produtos doados, volume total de beneficiários atendidos, volume total de reservas retiradas, e quantidade de beneficiários e doadores cadastrados na plataforma.| |  |
|RF-011| A aplicação deve gerar histórico das doações, e relatórios de métricas de doações para o doador, com quantidade de produtos doados, quantidade de reservas retiradas, e para o beneficiário com o status das suas reservas, e no histórico exibe o volume total itens recebidos.| |  |
|RF-012| A aplicação deve permitir ao doador delimitar no item doado a quantidade que cada classe de beneficiário pode retirar do item.| |Victor  |
|RF-013| A aplicação deve permitir que doador e receptor acessem o perfil um do outro para validação dos dados públicos.|Migrations\20260531224115_AdicionaBioUsuario.Designer.cs; PerfilPublicoViewModel.cs; Views\Usuarios\MeuPerfil.cshtml; Views\Usuarios\PerfilPublico.cshtml; Views\Usuarios\PerfilPublico.cshtml.cs; Views\Shared\_Layout.cshtml; Models\Usuario.cs | Paulo |
|RF-014| A aplicação deve permitir que doador e receptor avaliem um ao outro com 1 a 5 estrelas e comentários após a conclusão da retirada.| |  |
|RF-015| A aplicação deve disponibilizar um chat ou sistema de mensagens interno para comunicação direta e alinhamento entre doador e beneficiário.| Não Implementado | Não Implementado |

# Instruções de acesso

URL de acesso
Link: https://doalim.azurewebsites.net/

# Usuários com acessos restritos

- Administrador

Email: Admim@doalim.com

Senha: Admin@123

# Usuários com acessos

- Beneficiário teste

Email: benef@doalim.com 

Senha: Benef@123

- Doador teste

Email: doador@doalim.com 

Senha: Doador@123



Não deixe de informar o link onde a aplicação estiver disponível para acesso (por exemplo: https://adota-pet.herokuapp.com/src/index.html).

Se houver usuário de teste, o login e a senha também deverão ser informados aqui (por exemplo: usuário - admin / senha - admin).

