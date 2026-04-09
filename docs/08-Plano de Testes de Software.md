# Plano de Testes de Software

<p align="justify">
O plano de testes de software constitui um instrumento fundamental para garantir a qualidade e a confiabilidade de sistemas desenvolvidos, estabelecendo diretrizes, critérios e procedimentos para a verificação e validação das funcionalidades implementadas. Além disso, o plano contribui para a identificação precoce de falhas, promovendo maior eficiência no desenvolvimento e reduzindo riscos associados à implantação do software. Dessa forma, sua aplicação sistemática assegura maior conformidade com os requisitos estabelecidos e com as expectativas dos usuários finais.
 
Para este projeto foram definidos os seguintes casos de testes a serem aplicados:
</p>

| **Caso de Teste** 	| **CT01 – Cadastrar perfil** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-001 - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha. <br> RF-002 - A aplicação deve exigir o aceite digital obrigatório do "Termo de Responsabilidade" (baseado na Lei 14.016/2020) no momento do cadastro do doador. |
| Objetivo do Teste 	| Verificar se o usuário consegue se cadastrar na aplicação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Clicar em "Cadastre-se" na homepage <br> - Preencher os campos obrigatórios (Nome/Razão Social, CPF/CNPJ, E-mail, Senha, entre outros...) <br> - Informar se o cadastro está sendo realizado para um perfil de "doador" ou "beneficiário" preenchendo o campo respectivo <br> No caso do cadastro de doador, marcar o campo obrigatório referente ao aceite dos Termos de Responsabilidade <br> - Clicar em "Cadastrar" |
|Critério de Êxito | - O cadastro foi realizado com sucesso. |

| **Caso de Teste** 	| **CT02 – Efetuar Login** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-001 - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha. |
| Objetivo do Teste 	| Verificar se o usuário consegue entrar com sua conta cadastrada na aplicação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Clicar em "Login" na homepage <br> - Preencher os campos com as informações de acesso cadastradas <br> - Clicar em "Entrar" |
|Critério de Êxito | - O login foi realizado com sucesso. |

| **Caso de Teste** 	| **CT03 – Efetuar Logout** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-001 - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha. |
| Objetivo do Teste 	| Verificar se o usuário consegue sair da sua conta previamente logada. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login <br> - Clicar em "Logout" no cabeçalhos da página |
|Critério de Êxito | - O usuário foi desconectado do perfil logado. |

| **Caso de Teste** 	| **CT04 – Recuperar Senha** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-001 - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha. |
| Objetivo do Teste 	| Verificar o redirecionamento da senha para o email cadastrado. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Clicar em "Login" na homepage <br> - Clicar em "Recuperar senha" <br> - Preencher o campo referente ao email cadastrado <br> - Em seu correio eletrônico, verificar o recebimento do email com as instruções de recuperação, acessando o link fornecido <br> - Na página redirecionada, preencher os campos obrigatórios com nova senha |
|Critério de Êxito | - Receber o email com as instruções de recuperação <br> - Cadastro da nova senha realizado com sucesso. |

| **Caso de Teste** 	| **CT05 – Edição de Perfil** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-003 - A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador. |
| Objetivo do Teste 	| Verificar o usuário consegue alterar as informações de perfil. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com a conta previamente cadastrada <br> - Acessar o perfil através da foto de perfil no cabeçalho <br> - Clicar no íncone de edição ao lado dos campos permitidos para alteração <br> Clicar em "Salvar" |
|Critério de Êxito | - Conseguir salvar e visualizar as alterações nas informações do perfil |

| **Caso de Teste** 	| **CT06 – Envio de documentos para verificação** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-003 - A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador. |
| Objetivo do Teste 	| Verificar o usuário consegue alterar as informações de perfil. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com a conta previamente cadastrada <br> - Acessar o perfil através da foto de perfil no cabeçalho <br> - Anexar os documentos de Comprovação de Identificação através do campo destinado <br> Clicar em "Salvar" |
|Critério de Êxito | - Visualizar a mensagem de "arquivo anexado com êxito" |

| **Caso de Teste** 	| **CT07 – Visualização de histórico - Benificiário** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-003 - A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador. |
| Objetivo do Teste 	| Verificar se o beneficiário consegue visualizar seu histórico de solicitações de reserva. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com a conta de beneficiário <br> - A partir da homepage, clicar em "Visualizar histórico de requisição de reservas" <br> - Ao clicar em cada item do histórico, visualizar as informações da reserva |
|Critério de Êxito | - Visualizar as informações de cada requisição de reserva |

| **Caso de Teste** 	| **CT08 – Visualização de histórico - Doador** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-003 - A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador. |
| Objetivo do Teste 	| Verificar se o doador consegue visualizar seu histórico de produtos cadastrados para doação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com a conta de doador <br> - A partir da homepage, clicar em "Visualizar histórico de produtos cadastrados" <br> - Ao clicar em cada item do histórico, visualizar as informações do produto cadastrado para doação <br> - Caso haja uma requisição de reserva, conseguir acessar a página da reserva clicando na solicitação |
|Critério de Êxito | - Visualizar as informações de cada produto cadastrado <br> - Conseguir acessar páginas de solicitação de reserva |

| **Caso de Teste** 	| **CT09 – Cadastro de Produto** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-004 - A aplicação deve permitir que o doador cadastre itens para doação com as informações dos produtos. <br> RF-012 A aplicação deve permitir ao doador delimitar no item doado a quantidade que cada classe de beneficiário pode retirar do item. |
| Objetivo do Teste 	| Verificar se o usuário do tipo doador consegue cadastrar um produto para doação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com uma conta do tipo "doador" <br> - A partir do homepage, clicar em "Cadastar produto" <br> - Preencher os campos obrigatórios do formulário de cadastro do produto <br> - Indicar nos campos dedicados a divisão da quantidade destinada para cada tipo de usuário <br> - Clicar em "Salvar" |
|Critério de Êxito | - Produto registrado com êxito no banco de dados |

| **Caso de Teste** 	| **CT10 – Visualizar Vitrine** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-005 - A aplicação deve exibir uma vitrine em tempo real das doações disponíveis, permitindo que o beneficário possa realizar filtros. |
| Objetivo do Teste 	| Verificar se o usuário consegue visualizar produtos na vitrine |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - A partir da homepage, clicar em "buscar" <br> - Na barra de pesquisa da vitrine, inserir o termo exato ou aproximado do nome do produto desejado <br> Caso haja produto disponível para o termo pesquisado, visualizar lista de produtos |
|Critério de Êxito | - Visualizar produtos cadastrados para doação |

| **Caso de Teste** 	| **CT11 – Reserva de Produto** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-006 - A aplicação deve permitir que o beneficiário solicite e reserve uma doação, alterando o status do item no sistema para evitar que outra pessoa reserve o mesmo alimento. |
| Objetivo do Teste 	| Verificar se o usuário consegue reservar algum produto da vitrine |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo beneficiário <br> - Acessar a vitrine e buscar por um produto desejado <br> Selecionar um produto disponivel e clicar em "Solicitar Reservar" <br> Informar a quantidade solicitada no modal dedicado e clicar em "Reservar" |
|Critério de Êxito | - Produto desejado reservado com sucesso <br> Caso a quantidade se esgote, produto deve ser retirado da vitrine |

| **Caso de Teste** 	| **CT12 – Confirmação da Data de Retirada** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-009 - A aplicação deve notificar os usuários sobre aprovação ou recusa de reservas, lembretes de retirada e doações expiradas. |
| Objetivo do Teste 	| Verificar se o doador consegue confirmar a reserva solicitada pelo beneficiário, informando uma data de retirada |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - Acessar à página de proposta de reserva recebido, seja através do histórico de produtos cadastrados ou diretamente da homepage <br> - Clicar em uma das propostas recebidas <br> A partir do modal gerado, clicar em "Confirmar Reserva" <br> - Informar uma data disponível para a retirada do produto no campo destinado e clicar em "Notificar Beneficário" |
|Critério de Êxito | - Visualizar a mensagem de "Confirmação da reserva realizada com sucesso!" |

| **Caso de Teste** 	| **CT13 – Confirmação da Doação Realizada** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-007 - A aplicação deve permitir que o doador valide a entrega da doação através de um código numérico ou QR Code apresentado pelo receptor no momento da retirada. |
| Objetivo do Teste 	| Verificar se o doador consegue confirmar a finalizaçao da doação à partir do código recebido e informado pelo beneficiário |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - Acessar à página de proposta de reserva recebido, seja através do histórico de produtos cadastrados ou diretamente da homepage <br> - Clicar em uma das propostas recebidas <br> A partir do modal gerado, clicar em "Concluir Doação" <br> - Informar o código fornecido pelo beneficiário |
|Critério de Êxito | - Doação finalizada com sucesso |

| **Caso de Teste** 	| **CT14 – Moderação de Usuários ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-008 - A aplicação deve possuir um painel administrativo para moderação de conteúdo e gerenciamento de usuários. |
| Objetivo do Teste 	| Verificar se o perfil de administrador consegue aprovar documentação enviadas pelo doador e beneficiário |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo administrador, fornecida pelo desenvolvedor <br> - A partir do painel de administrativo (homepage) clicar em "lista de usuários" <br> - Ao clicar em cada uduário, conseguir visualizar as informações do mesmo <br> - Baixar as documentações fornecidas no ícone ao lado do campo dedicado <br> - Aceitar ou recusar documentação fornecida <br> Em caso de recusa, digitar uma mensagem informando ao usuário a causa <br> Clicar em "Salvar". |
|Critério de Êxito | - Conseguir acessar a documentação fornecida pelo usuário <br> - Aprovar ou rejeitar documentação com êxito |

| **Caso de Teste** 	| **CT15 – Moderação de Métricas ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-008 - A aplicação deve possuir um painel administrativo para moderação de conteúdo e gerenciamento de usuários. |
| Objetivo do Teste 	| Verificar se o perfil de administrador consegue aprovar documentação enviadas pelo doador e beneficiário |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo administrador, fornecida pelo desenvolvedor <br> - A partir do painel de administrativo (homepage) clicar em "Métricas da Plataforma" <br> - Preencher os filtros disponíveis <br> - Exportar um relatório com os dados filtrados através de um botão dedicado. |
|Critério de Êxito | - Manipular corretamente os filtros disponíveis com exibição de dados condizente <br> - Conseguir exportar com êxito um relatório com as informações filtradas |

| **Caso de Teste** 	| **CT16 – Recebimento de Notificações** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-009 - A aplicação deve notificar os usuários sobre aprovação ou recusa de reservas, lembretes de retirada e doações expiradas. |
| Objetivo do Teste 	| Verificar se o doador ou beneficiário estão recebendo notificações do status de uma doação/reserva |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login <br> - A partir da homepage clicar no ícone de notificações no cabeçalho <br> - Conseguir visualizar todas as notificações na página de "histórico de notificações"
|Critério de Êxito | - Ter acesso à pagina de notificações, visualizando todas as notificações recebidas |

| **Caso de Teste** 	| **CT17 – Relatório de Impacto - Doador ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-010 - A aplicação deve gerar relatórios de impacto para o doador, exibindo o volume total doado e a redução estimada de CO₂ gerada por evitar o descarte. |
| Objetivo do Teste 	| Verificar se o doador consegue emitir um relatório de impacto socioambiental |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - A partir da homepage clicar em "Painel de Impacto Ambiental" <br> - Na página acessada, conseguir visualizar as métricas <br>  - Manipular os filtros conforme necessidade <br> - Clicar em "Exportar" bara baixar um relatório de impacto com as métricas do usuário. |
|Critério de Êxito | - Conseguir visualizar e exportar as métricas de impacto socioambiental |

| **Caso de Teste** 	| **CT18 – Relatório de Impacto - Beneficiário ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-011 - A aplicação deve gerar relatórios de impacto para o beneficiário, exibindo o volume total itens recebidos. |
| Objetivo do Teste 	| Verificar se o beneficiário consegue emitir um relatório de impacto socioambiental |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo beneficiário <br> - A partir da homepage clicar em "Doações Recebidas" <br> - A partir da página de doações, clicar em "Métricas do Beneficiário" <br> - Na página acessada, conseguir visualizar as métricas <br> - Manipular os filtros conforme necessidade <br> - Clicar em "Exportar" bara baixar um relatório de impacto com as métricas do usuário. |
|Critério de Êxito | - Conseguir visualizar e exportar as métricas de impacto socioambiental |

| **Caso de Teste** 	| **CT19 – Acessos ao perfil público do beneficiário** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-013 - 	A aplicação deve permitir que doador e receptor acessem o perfil um do outro para visualização dos dados públicos. |
| Objetivo do Teste 	| Verificar se os usuários conseguem visualizar o perfil público dos demais usuários |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - Acessar à página de proposta de reserva recebido, seja através do histórico de produtos cadastrados ou diretamente da homepage <br> - Clicar em uma das propostas recebidas <br> A partir do modal gerado, clicar no ícone do beneficiário solicitante |
|Critério de Êxito | - Acesso ao perfil público do usuário beneficiário solicitante |

| **Caso de Teste** 	| **CT20 – Acessos ao perfil público do doador** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-013 - 	A aplicação deve permitir que doador e receptor acessem o perfil um do outro para visualização dos dados públicos. |
| Objetivo do Teste 	| Verificar se os usuários conseguem visualizar o perfil público dos demais usuários |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo beneficiário <br> - Acessar a vitrine e buscar por um produto desejado <br> Selecionar um produto disponivel e clicar no ícone do doador <br> Alternativamente, a partir do histórico de requisição de reservas, clicar em um item do histórico <br> - Nas informações da reserva, clicar no ícone do doador |
|Critério de Êxito | - Acesso ao perfil público do usuário doador ofertante |


=============================== APAGAR DAQUI PARA BAIXO AO FINALIZAR ===============================

<span style="color:red">Pré-requisitos: <a href="2-Especificação do Projeto.md"> Especificação do Projeto</a></span>, <a href="3-Projeto de Interface.md"> Projeto de Interface</a>

Apresente os cenários de testes utilizados na realização dos testes da sua aplicação. Escolha cenários de testes que demonstrem os requisitos sendo satisfeitos.

Não deixe de enumerar os casos de teste de forma sequencial e de garantir que o(s) requisito(s) associado(s) a cada um deles está(ão) correto(s) - de acordo com o que foi definido na seção "2 - Especificação do Projeto". 

Por exemplo:
 
| **Caso de Teste** 	| **CT01 – Cadastrar perfil** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-00X - A aplicação deve apresentar, na página principal, a funcionalidade de cadastro de usuários para que esses consigam criar e gerenciar seu perfil. |
| Objetivo do Teste 	| Verificar se o usuário consegue se cadastrar na aplicação. |
| Passos 	| - Acessar o navegador <br> - Informar o endereço do site https://adota-pet.herokuapp.com/src/index.html<br> - Clicar em "Criar conta" <br> - Preencher os campos obrigatórios (e-mail, nome, sobrenome, celular, CPF, senha, confirmação de senha) <br> - Aceitar os termos de uso <br> - Clicar em "Registrar" |
|Critério de Êxito | - O cadastro foi realizado com sucesso. |
|  	|  	|
| Caso de Teste 	| CT02 – Efetuar login	|
|Requisito Associado | RF-00Y	- A aplicação deve possuir opção de fazer login, sendo o login o endereço de e-mail. |
| Objetivo do Teste 	| Verificar se o usuário consegue realizar login. |
| Passos 	| - Acessar o navegador <br> - Informar o endereço do site https://adota-pet.herokuapp.com/src/index.html<br> - Clicar no botão "Entrar" <br> - Preencher o campo de e-mail <br> - Preencher o campo da senha <br> - Clicar em "Login" |
|Critério de Êxito | - O login foi realizado com sucesso. |

 
> **Links Úteis**:
> - [IBM - Criação e Geração de Planos de Teste](https://www.ibm.com/developerworks/br/local/rational/criacao_geracao_planos_testes_software/index.html)
> - [Práticas e Técnicas de Testes Ágeis](http://assiste.serpro.gov.br/serproagil/Apresenta/slides.pdf)
> -  [Teste de Software: Conceitos e tipos de testes](https://blog.onedaytesting.com.br/teste-de-software/)
> - [Criação e Geração de Planos de Teste de Software](https://www.ibm.com/developerworks/br/local/rational/criacao_geracao_planos_testes_software/index.html)
> - [Ferramentas de Test para Java Script](https://geekflare.com/javascript-unit-testing/)
> - [UX Tools](https://uxdesign.cc/ux-user-research-and-user-testing-tools-2d339d379dc7)
