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
|Critério de Êxito | - O cadastro foi realizado com sucesso. <br>- Para o perfil Doador: o aceite do Termo de Responsabilidade é exigido obrigatoriamente.<br> |

| **Caso de Teste** 	| **CT02 – Efetuar Login** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-001 - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha. |
| Objetivo do Teste 	| Verificar se o usuário consegue entrar com sua conta cadastrada na aplicação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Clicar em "Login" na homepage <br> - Preencher os campos com as informações de acesso cadastradas <br> - Clicar em "Entrar" |
|Critério de Êxito | - O login foi realizado com sucesso e o usuário é redirecionado à homepage autenticada. |

| **Caso de Teste** 	| **CT03 – Efetuar Logout** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-001 - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha. |
| Objetivo do Teste 	| Verificar se o usuário consegue sair da sua conta previamente logada. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login <br> - Clicar em "Logout" no cabeçalho da página |
|Critério de Êxito | - O usuário foi desconectado do perfil logado e redirecionado à tela inicial. |

| **Caso de Teste** 	| **CT04 – Recuperar Senha** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-001 - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha. |
| Objetivo do Teste 	| Verificar o redirecionamento da senha para o email cadastrado. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Clicar em "Login" na homepage <br> - Clicar em "Recuperar senha" <br> - Preencher o campo referente ao email cadastrado <br> - Em seu correio eletrônico, verificar o recebimento do email com as instruções de recuperação, acessando o link fornecido <br> - Na página redirecionada, preencher os campos obrigatórios com nova senha |
|Critério de Êxito | - Receber o email com as instruções de recuperação <br> - Cadastro da nova senha realizado com sucesso. |

| **Caso de Teste** 	| **CT05 – Edição de Perfil** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-003 - A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador. |
| Objetivo do Teste 	| Verificar dr o usuário consegue alterar as informações de perfil. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência. <br> - Efetuar o login com a conta previamente cadastrada. <br> - Acessar o perfil através da foto de perfil no cabeçalho. <br> - Clicar no íncone de edição ao lado dos campos permitidos para alteração. <br> - Clicar em "Salvar" |
|Critério de Êxito | - As alterações nas informações do perfil foram salvas e exibidas corretamente. |

| **Caso de Teste** 	| **CT06 – Envio de documentos para verificação** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-003 - A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador. |
| Objetivo do Teste 	| Verificar se o usuário consegue enviar documentos para verificação pelo administrador. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com a conta previamente cadastrada <br> - Acessar o perfil através da foto de perfil no cabeçalho <br> - Anexar os documentos de Comprovação de Identificação através do campo destinado <br> - Clicar em "Salvar" |
|Critério de Êxito | - Visualizar a mensagem de "arquivo anexado com êxito." <br> O documento enviado fica disponível para revisão pelo administrador.<br>|

| **Caso de Teste** 	| **CT07 – Visualização de histórico - Beneficiário** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-006 – A aplicação deve permitir que o beneficiário solicite e reserve uma doação, alterando o status do item no sistema para evitar que outra pessoa reserve o mesmo alimento. |
| Objetivo do Teste 	| Verificar se o beneficiário consegue visualizar seu histórico de solicitações de reserva. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com a conta de beneficiário <br> - A partir da homepage, clicar em "Visualizar histórico de requisição de reservas" <br> - Ao clicar em cada item do histórico, visualizar as informações da reserva |
|Critério de Êxito | - Visualizar as informações de cada requisição de reserva são exibidas corretamente. |

| **Caso de Teste** 	| **CT08 – Visualização de histórico - Doador** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-004 – A aplicação deve permitir que o doador cadastre itens para doação com as informações dos produtos. |
| Objetivo do Teste 	| Verificar se o doador consegue visualizar seu histórico de produtos cadastrados para doação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com a conta de doador <br> - A partir da homepage, clicar em "Visualizar histórico de produtos cadastrados" <br> - Ao clicar em cada item do histórico, visualizar as informações do produto cadastrado para doação <br> - Caso haja uma requisição de reserva, conseguir acessar a página da reserva clicando na solicitação |
|Critério de Êxito | - Visualizar as informações de cada produto cadastrado são exibidas corretamente. <br> - Conseguir acessar as páginas de solicitação de reserva vinculadas. |

| **Caso de Teste** 	| **CT09 – Cadastro de Produto** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-004 - A aplicação deve permitir que o doador cadastre itens para doação com as informações dos produtos. <br> RF-012 A aplicação deve permitir ao doador delimitar no item doado a quantidade que cada classe de beneficiário pode retirar do item. |
| Objetivo do Teste 	| Verificar se o usuário do tipo doador consegue cadastrar um produto para doação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Efetuar o login com uma conta do tipo "doador" <br> - A partir do homepage, clicar em "Cadastrar produto" <br> - Preencher os campos obrigatórios do formulário de cadastro do produto <br> - Indicar nos campos dedicados a divisão da quantidade destinada para cada tipo de usuário <br> - Clicar em "Salvar" |
|Critério de Êxito | - Produto registrado com êxito no banco de dados. |

| **Caso de Teste** 	| **CT10 – Visualizar Vitrine** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-005 - A aplicação deve exibir uma vitrine em tempo real das doações disponíveis, permitindo que o beneficário possa realizar filtros. |
| Objetivo do Teste 	| Verificar se o usuário consegue visualizar produtos na vitrine |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - A partir da homepage, clicar em "buscar" <br> - Na barra de pesquisa da vitrine, inserir o nome do produto desejado. <br> - Verificar se a lista de produtos disponíveis é exibida. <br> - Aplicar os filtros disponíveis (categoria, data de validade, localização, etc.).<br> - Verificar se os resultados são atualizados conforme os filtros aplicados. <br>|
|Critério de Êxito | - Os produtos cadastrados para doação são exibidos na vitrine. <br> - Os filtros alteram os resultados exibidos de forma condizente com os critérios selecionados. <br> |

| **Caso de Teste** 	| **CT11 – Reserva de Produto** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-006 - A aplicação deve permitir que o beneficiário solicite e reserve uma doação, alterando o status do item no sistema para evitar que outra pessoa reserve o mesmo alimento. |
| Objetivo do Teste 	| Verificar se o usuário consegue reservar um produto da vitrine e se o status do item é atualizado corretamente |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo beneficiário <br> - Acessar a vitrine e buscar por um produto desejado <br> - Selecionar um produto disponivel e clicar em "Solicitar Reserva" <br> - Informar a quantidade solicitada no modal dedicado e clicar em "Reservar" |
|Critério de Êxito | - Produto desejado reservado com sucesso. <br> - O status do item é atualizado imediatamente no sistema. <br> - Caso a quantidade se esgote, o produto é removido da vitrine e indisponibilizado para outros beneficiários. <br>|

| **Caso de Teste** 	| **CT12 – Confirmação da Data de Retirada** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-009 - A aplicação deve notificar os usuários sobre aprovação ou recusa de reservas, lembretes de retirada e doações expiradas. |
| Objetivo do Teste 	| Verificar se o doador consegue confirmar a reserva solicitada pelo beneficiário, informando uma data de retirada |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - Acessar à página de proposta de reserva recebido, seja através do histórico de produtos cadastrados ou diretamente da homepage <br> - Clicar em uma das propostas recebidas <br> - A partir do modal gerado, clicar em "Confirmar Reserva" <br> - Informar uma data disponível para a retirada do produto no campo destinado e clicar em "Notificar Beneficário" |
|Critério de Êxito | - A mensagem "Confirmação da reserva realizada com sucesso!" é exibida.<br> - O beneficiário recebe notificação com a data de retirada confirmada.<br> |

| **Caso de Teste** 	| **CT13 – Confirmação da Doação Realizada** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-007 - A aplicação deve permitir que o doador valide a entrega da doação através de um código numérico ou QR Code apresentado pelo receptor no momento da retirada. |
| Objetivo do Teste 	| Verificar se o doador consegue confirmar a finalizaçao da doação à partir do código recebido e informado pelo beneficiário |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - Acessar à página de proposta de reserva recebido, seja através do histórico de produtos cadastrados ou diretamente da homepage <br> - Clicar em uma das propostas recebidas <br> - A partir do modal gerado, clicar em "Concluir Doação" <br> - Informar o código fornecido pelo beneficiário |
|Critério de Êxito | - Doação finalizada com sucesso. <br> - O status da reserva é atualizado para "Concluída" no sistema. <br> |

| **Caso de Teste** 	| **CT14 – Moderação de Usuários ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-008 - A aplicação deve possuir um painel administrativo para moderação de conteúdo e gerenciamento de usuários. |
| Objetivo do Teste 	| Verificar se o perfil de administrador consegue aprovar documentação enviadas pelo doador e beneficiário |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo administrador, fornecida pelo desenvolvedor <br> - A partir do painel de administrativo (homepage) clicar em "lista de usuários" <br> - Ao clicar em cada uduário, conseguir visualizar as informações do mesmo <br> - Baixar as documentações fornecidas no ícone ao lado do campo dedicado <br> - Aceitar ou recusar documentação fornecida <br> Em caso de recusa, digitar uma mensagem informando ao usuário a causa <br> - Clicar em "Salvar". |
|Critério de Êxito | - É possível acessar e baixar a documentação fornecida pelo usuário.  <br> - A aprovação ou rejeição da documentação é registrada com êxito. <br> - Em caso de recusa, o usuário recebe a mensagem com a justificativa.  |

| **Caso de Teste** 	| **CT15 – Moderação de Métricas ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-008 - A aplicação deve possuir um painel administrativo para moderação de conteúdo e gerenciamento de usuários. |
| Objetivo do Teste 	| Verificar se o perfil de administrador consegue aprovar documentação enviadas pelo doador e beneficiário |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo Administrador, fornecida pelo desenvolvedor <br> - A partir do painel de administrativo clicar em "Métricas da Plataforma". <br> - Preencher os filtros disponíveis conforme necessidade. <br> - Exportar um relatório com os dados filtrados através de um botão dedicado. |
|Critério de Êxito | - Os filtros funcionam corretamente, exibindo dados condizentes com os critérios selecionados. <br> - Conseguir exportar com êxito um relatório com as informações filtradas. |

| **Caso de Teste** 	| **CT16 – Recebimento de Notificações** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-009 - A aplicação deve notificar os usuários sobre aprovação ou recusa de reservas, lembretes de retirada e doações expiradas. |
| Objetivo do Teste 	| Verificar se o doador ou beneficiário estão recebendo notificações do status de uma doação/reserva |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login como Doador ou Beneficiário <br> - Provocar diferentes eventos (aprovação de reserva, recusa, lembrete de retirada) através dos fluxos de teste anteriores. <br> - Clicar no ícone de notificações no cabeçalho. <br> - Verificar o histórico de notificações recebidas |   
|Critério de Êxito | - As notificações de aprovação, recusa e lembrete de retirada são exibidas corretamente na página de histórico de notificações. <br> - Doações expiradas geram notificação para o doador responsável. |

| **Caso de Teste** 	| **CT17 – Relatório de Impacto - Doador ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-010 - A aplicação deve gerar relatórios de impacto para o doador, exibindo o volume total doado e a redução estimada de CO₂ gerada por evitar o descarte. |
| Objetivo do Teste 	| Verificar se o doador consegue emitir um relatório de impacto socioambiental |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - A partir da homepage clicar em "Painel de Impacto Ambiental" <br> - Visualizar as métricas exibidas. <br>  - Manipular os filtros conforme necessidade <br> - Clicar em "Exportar" para baixar um relatório de impacto com as métricas do usuário. |
|Critério de Êxito | - As métricas de impacto socioambiental são exibidas corretamente. <br> - O relatório é exportado com êxito. |

| **Caso de Teste** 	| **CT18 – Relatório de Impacto - Beneficiário ** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-011 - A aplicação deve gerar relatórios de impacto para o beneficiário, exibindo o volume total itens recebidos. |
| Objetivo do Teste 	| Verificar se o beneficiário consegue emitir um relatório de impacto socioambiental |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo beneficiário <br> - A partir da homepage clicar em "Doações Recebidas" <br> - Clicar em "Métricas do Beneficiário" <br> - Visualizar as métricas exibidas na página <br> - Manipular os filtros conforme necessidade <br> - Clicar em "Exportar" para baixar um relatório de impacto com as métricas do usuário. |
|Critério de Êxito | - As métricas de impacto são exibidas corretamente. <br> - O relatório é exportado com êxito. |

| **Caso de Teste** 	| **CT19 – Acessos ao perfil público do beneficiário** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-013 - 	A aplicação deve permitir que doador e receptor acessem o perfil um do outro para visualização dos dados públicos. |
| Objetivo do Teste 	| Verificar se os usuários conseguem visualizar o perfil público dos demais usuários |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo doador <br> - Acessar à página de proposta de reserva recebido, seja através do histórico de produtos cadastrados ou diretamente da homepage <br> - Clicar em uma das propostas recebidas <br> A partir do modal gerado, clicar no ícone do beneficiário solicitante |
|Critério de Êxito | - O perfil público do beneficiário solicitante é exibido corretamente. |

| **Caso de Teste** 	| **CT20 – Acessos ao perfil público do doador** 	|
|:---:	|:---:	|
|	Requisito Associado 	| RF-013 - 	A aplicação deve permitir que doador e receptor acessem o perfil um do outro para visualização dos dados públicos. |
| Objetivo do Teste 	| Verificar se os usuários conseguem visualizar o perfil público dos demais usuários |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login com uma conta do tipo beneficiário <br> - Acessar a vitrine e buscar por um produto desejado <br> Selecionar um produto disponivel e clicar no ícone do doador <br> Alternativamente, a partir do histórico de requisição de reservas, clicar em um item do histórico <br> - Nas informações da reserva, clicar no ícone do doador |
|Critério de Êxito | - O perfil público do doador ofertante é exibido corretamente. |

| Caso de Teste | CT21 – Avaliação de Usuário (Doador/Beneficiário) |
|:---:	|:---:	|
|	Requisito Associado 	| RF-014 - A aplicação deve permitir que doador e receptor avaliem um ao outro com 1 a 5 estrelas e comentários após a conclusão da retirada. |
| Objetivo do Teste 	| Verificar se o sistema permite o envio de nota e comentário após o encerramento da doação. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login como Doador ou Beneficiário <br> - Acessar uma doação já marcada como "Concluída" através do histórico <br> - Clicar no botão "Avaliar Experiência" <br> - Selecionar uma nota (ex: 1 a 5 estrelas) <br> - Escrever um comentário curto <br> - Clicar em "Enviar Avaliação" |
|Critério de Êxito | - A avaliação é salva com sucesso <br> - A avaliação passa a ser exibida no perfil público do usuário avaliado |

| Caso de Teste | CT22 – Comunicação via Chat Interno |
|:---:	|:---:	|
|	Requisito Associado 	| RF-015 - A aplicação deve disponibilizar um chat ou sistema de mensagens interno para comunicação direta e alinhamento entre doador e beneficiário. |
| Objetivo do Teste 	| Verificar se as mensagens são enviadas e recebidas corretamente entre as duas partes. |
| Passos 	| - Acessar o endereço da aplicação através do navegador de preferência <br> - Realizar o login como Beneficiário <br> - Acessar uma reserva ativa <br> - Clicar no ícone de "Chat" ou "Enviar Mensagem" <br> - Digitar uma mensagem de texto e enviar <br> - Acessar a área de mensagens/notificações para verificar o recebimento |
|Critério de Êxito | - A mensagem é entregue em tempo real <br> - A notificação é exibida corretamente no ícone de mensagens do destinatário |

| Caso de Teste | CT23 – Acesso a APIs externas|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-001 - A aplicação deve conseguir acessar API's |
| Objetivo do Teste 	| Verificar se o sistema realiza comunicação bem-sucedida com serviços externos. |
| Passos 	| - Acessar a página com o formulário que consome a API (ex: preenchimento de CEP no cadastro) <br> - Inserir um dado válido <br> - Abrir as ferramentas de desenvolvedor do navegador (Aba Network/Rede) <br> - Clicar em buscar ou fora do campo |
|Critério de Êxito | - A requisição retorna status OK. <br> - Os dados retornados pela API preenchem a interface corretamente. |

| Caso de Teste | CT24 – Tecnologias utilizadas no desenvolvimento|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-002 - A aplicação deve ser desenvolvida utilizando a linguagem C# no back-end e a tríade JavaScript, HTML e CSS (com framework Bootstrap) no front-end. |
| Objetivo do Teste 	| Verificar por inspeção se as tecnologias definidas estão sendo utilizadas no projeto. |
| Passos 	| - Acessar o Visual Studio ou repositório do projeto <br> - Verificar o nome e extensão dos arquivos de back-end <br> - Verificar os arquivos de front-end e a importação das bibliotecas Bootstrap. |
|Critério de Êxito | - Os arquivos do servidor possuem extensão .cs (C#). <br> - Os arquivos de interface utilizam extensões .html, .css e .js. <br> - O Bootstrap está devidamente importado e aplicado nas páginas. |

| Caso de Teste | CT25 – Conformidade com LGPD|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-003 - A aplicação deve estar em total conformidade com a LGPD, garantindo o tratamento seguro de dados sensíveis de famílias cadastradas e políticas claras de privacidade. |
| Objetivo do Teste 	| Verificar a presença de termos claros de consentimento e ferramentas de privacidade. |
| Passos 	| - Acessar o site e verificar aviso de Cookies <br> - Acessar a tela de cadastro e identificar a "Política de Privacidade" e os Termos de Responsabilidade <br> - Logar no sistema e acessar as configurações do perfil <br> - Localizar a opção de exclusão ou solicitação de remoção de dados. |
|Critério de Êxito | - Os documentos legais estão disponíveis para aceite no momento do cadastro. <br> - O botão de solicitação de remoção de dados está funcional e acessível. |

| Caso de Teste | CT26 – Analisar responsividade da aplicação (Mobile-first)|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-004 - A aplicação deve possuir uma interface "mobile-first", sendo responsiva e adaptável a telas de celulares, considerando que muitos receptores usarão o sistema via smartphone. |
| Objetivo do Teste 	| Verificar a responsividade da aplicação em telas menores. |
| Passos 	| - Acessar a URL da aplicação <br> - Redimensionar a janela do navegador (ou usar o F12 em modo mobile) para dimensões de um smartphone <br> - Navegar pelas páginas da vitrine, perfil e reserva de doações |
|Critério de Êxito | - A interface não apresenta falhas de posicionamento, quebra de botões ou necessidade de rolagem horizontal. <br> - Todos os elementos interativos são acessíveis e utilizáveis na versão mobile. |

| Caso de Teste | CT27 – Banco de dados utilizando SQL Server|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-008 - A aplicação deve utilizar o Microsoft SQL Server para armazenamento de dados. |
| Objetivo do Teste 	| Verificar o SGBD utilizado para a persistência de dados. |
| Passos 	| - Acessar as configurações do back-end <br> - Mostrar a string de conexão configurada <br> - Acessar a URL da aplicação e cadastrar um produto para doação <br> - Executar uma consulta (SELECT) através do SQL |
|Critério de Êxito | - A string de conexão aponta para o provedor SQL Server. <br> - O cadastro do produto aparece imediatamente na tabela correta do banco de dados |

| Caso de Teste | CT28 – Prevenção de double booking|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-005 – A aplicação deve garantir transações no banco de dados e evitar "double booking" (reservas simultâneas da mesma doação). |
| Objetivo do Teste 	| Verificar se o sistema impede que dois beneficiários reservem o mesmo item simultaneamente. |
| Passos 	| - Acessar a aplicação em dois navegadores distintos (ou abas anônimas) com duas contas de beneficiário diferentes. <br> - Em ambas as sessões, localizar o mesmo produto com quantidade limitada na vitrine <br> - Tentar efetuar a reserva simultânea do mesmo item nas duas sessões ao mesmo tempo. |
|Critério de Êxito | - Apenas uma das reservas é confirmada com sucesso. <br> - A segunda tentativa recebe mensagem de indisponibilidade ou estoque esgotado. <br> - O status do item é atualizado corretamente, sem duplicidade de reservas.|

| Caso de Teste | CT29 – Disponibilidade da aplicação (24/7)|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-006 – A aplicação deve estar disponível 24 horas por dia, 7 dias por semana. |
| Objetivo do Teste 	| Verificar se a aplicação permanece acessível e funcional fora do horário comercial. |
| Passos 	| - Acessar a URL da aplicação em horários variados (madrugada, fim de semana, feriados). <br> -Realizar o login com uma conta válida. <br> - Navegar pelas principais funcionalidades: vitrine, reserva e perfil. <br> - Verificar o tempo de resposta e a ausência de erros de servidor. |
|Critério de Êxito | - A aplicação responde normalmente em todos os horários testados. <br> - Nenhuma funcionalidade principal apresenta indisponibilidade fora do horário comercial. |

| Caso de Teste | CT30 – Compatibilidade entre navegadores|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-007 – A aplicação deve ser compatível com os principais navegadores (Chrome, Firefox e Edge). |
| Objetivo do Teste 	| Verificar se as funcionalidades principais funcionam corretamente nos três navegadores exigidos. |
| Passos 	| - AceAcessar a aplicação no Google Chrome e executar o fluxo: cadastro, login, busca na vitrine e reserva de produto. <br> - Repetir o mesmo fluxo no Mozilla Firefox. <br> - Repetir o mesmo fluxo no Microsoft Edge. <br> Registrar qualquer falha visual ou funcional identificada em cada navegador. |
|Critério de Êxito | - Nenhum dos três navegadores apresenta falha visual (layout quebrado) ou funcional (ação que não conclui). <br> - A experiência do usuário é equivalente nos três ambientes. |

| Caso de Teste | CT31 – Desempenho da vitrine|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-010 – A aplicação deve ter um tempo de resposta inferior a 5 segundos para buscas e listagens de doações na vitrine em condições normais de tráfego |
| Objetivo do Teste 	| Verificar se a busca e a listagem de doações na vitrine respondem em menos de 5 segundos. |
| Passos 	| - Acessar a vitrine da aplicação. <br> - Abrir as ferramentas de desenvolvedor do navegador (F12), aba Network/Rede. <br> - Realizar uma busca por produto. <br> - Analisar o tempo de resposta das requisições disparadas.|
|Critério de Êxito | - O tempo de resposta de cada requisição de busca e listagem é inferior a 5 segundos <br> - A interface exibe os resultados sem travamento ou carregamento prolongado. |

| Caso de Teste | CT32 – Acessibilidade básica (WCAG)|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-011 – A aplicação deve seguir diretrizes básicas de acessibilidade (WCAG), possuindo bom contraste de cores e navegação por teclado. |
| Objetivo do Teste 	| Verificar o SGBD utilizado para a persistência de dados. |
| Passos 	| - Acessar a aplicação nas páginas de cadastro, vitrine e perfil. <br> - Navegar pelas ações principais utilizando apenas as teclas Tab e Enter (sem mouse). <br> - Abrir as ferramentas de desenvolvedor do Chrome e executar a análise de Acessibilidade via Lighthouse. |
|Critério de Êxito | - As ações principais (login, busca, reserva) são completáveis via teclado sem necessidade de mouse. <br> - A pontuação de acessibilidade no Lighthouse é igual ou superior a 70. Não há elementos sem contraste mínimo entre texto e fundo. |

| Caso de Teste | CT33 – Segurança de senhas e protocolo HTTPS|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-012 – A aplicação deve garantir a segurança dos dados utilizando senhas com hash forte e comunicação via protocolo TLS/HTTPS. |
| Objetivo do Teste 	| Verificar se as senhas são armazenadas com hash e se toda comunicação ocorre via HTTPS. |
| Passos 	| - Acessar o banco de dados e consultar o campo de senha de um usuário cadastrado. <br> - Verificar na barra de endereço do navegador se a URL utiliza "https://". <br> - Abrir as ferramentas de desenvolvedor (aba Network) e confirmar se nenhuma requisição trafega via HTTP simples. |
|Critério de Êxito | - O campo de senha no banco de dados contém um hash irreconhecível, não o texto original. <br> - Todas as requisições da aplicação utilizam HTTPS. <br> - Nenhuma informação sensível é transmitida em texto puro. |

| Caso de Teste | CT34 – Carga simultânea de acessos|
|:---:	|:---:	|
|	Requisito Associado 	| RNF-009 – A aplicação deve ter capacidade de receber múltiplos acessos simultâneos. |
| Objetivo do Teste 	| Verificar se a aplicação suporta múltiplos acessos simultâneos sem degradação significativa de desempenho. |
| Passos 	| - Utilizar uma ferramenta de teste de carga (ex: Apache JMeter, Postman Runner ou k6). <br> - Configurar uma simulação com 20 requisições simultâneas ao endpoint da vitrine. <br> Executar o teste e monitorar os tempos de resposta e possíveis erros retornados. |
|Critério de Êxito | - Todas as requisições retornam resposta válida (status 200 OK). <br> - O tempo de resposta médio permanece dentro do limite definido pelo RNF-010 (abaixo de 5 segundos). <br> - Nenhum erro de servidor (5xx) é retornado durante a execução do teste. |
