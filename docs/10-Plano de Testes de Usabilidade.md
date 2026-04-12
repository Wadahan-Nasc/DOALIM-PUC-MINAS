# Plano de Testes de Usabilidade

Testes de usabilidade possuem relação com a experiência do usuário em utilizar a aplicação, visando verificar o seu funcionamento por meio da observação. Com esta técnica é possível controlar se a aplicação atende aos requisitos e solicitações realizadas, certificando a qualidade da interface.

Através da observação dos testes realizados é possível identificar diferentes comportamentos e reações dos participantes diante das diferentes telas navegadas da aplicação interativa.

Mediante dados coletados e métricas é feita uma análise dos resultados com o intuito de identificar melhorias e problemas, a fim de fornecer uma melhor experiência do usuário.

Para este projeto será realizado o modelo de testes remoto e moderado com observação da execução das tarefas realizadas pelos participantes por meio da ferramenta Lookback (ou plataformas de videoconferência similares). O planejamento dos testes a serem executados com os participantes é descrito a seguir: objetivos, método e modelo utilizado, seleção dos participantes, roteiro das tarefas a serem desempenhadas pelos usuários e análise.

Os testes serão realizados em duas etapas, sendo a primeira etapa realizada com tarefas testes específicas tendo a aplicação ainda em desenvolvimento e executando as funcionalidades essenciais, a fim de identificar erros e sugestões de melhoria, e a segunda etapa os participantes simularão cenários testes tendo a aplicação finalizada e apresentando todas as suas funcionalidades.

# 1. Objetivos

Os seguintes objetivos foram definidos com a finalidade de possibilitar uma experiência positiva do usuário:

> - A aplicação deve possibilitar que o usuário navegue de maneira intuitiva;
> - As telas devem ser simples e compreensíveis para que o usuário realize suas tarefas de modo prático;
> - A aplicação deve disponibilizar meios para que a execução das tarefas do usuário seja factível, considerando a diversidade de letramento digital do público-alvo (especialmente famílias vulneráveis).

Por meio dos testes será possível identificar problemas e o modo como os usuários interagem com a aplicação, respondendo deste modo questionamentos que visam a sua melhoria, são eles:

> - Os usuários navegam com facilidade e de modo intuitivo pelos diversos módulos da aplicação?
> - Os usuários conseguem compreender as telas acessadas e com isso executam a tarefa de modo rápido?
> - Os usuários cometem erros ao executar as tarefas (ex: ao reservar um produto ou validar um código de entrega)?
> - Existem obstáculos que impossibilitam a conclusão das tarefas? Se sim, quais obstáculos são esses?
> - Qual o tempo de resposta e como os usuários reagem a aplicação?
> - Usuários com baixo letramento digital conseguem concluir as tarefas essenciais sem assistência?

# 2. Método e modelo utilizado

Para este projeto em consideração, foi definido para aplicação dos testes o modelo remoto e moderado com método de experimentação e observação.

Buscando reproduzir um cenário coerente com a realidade, os usuários devem acessar a aplicação de qualquer dispositivo eletrônico independente da sua localização. Neste caso, o melhor modelo para observar o comportamento dos usuários durante a interação com o sistema é o modelo remoto, pois sendo a distância o participante pode realizar em seu ambiente natural (ex: o comerciante em sua padaria, o beneficiário em sua residência).

Além disso, este modelo oferece um custo baixo para aplicação dos testes, já que não se faz necessário a locação de um estabelecimento e gastos com pessoal e materiais necessários, proporciona testar 100% da aplicação e projetos, não interfere na velocidade do desenvolvimento e para o usuário é cômodo e prático.

# 3. Participantes

Serão selecionados usuários de acordo com as personas já estabelecidas para o projeto Doalim: Doador PJ (Comerciante), Doador PF (Cidadão Consciente), Receptor PJ (Gestora Social de ONG), Receptor PF (Chefe de Família em vulnerabilidade) e Administrador.

Participarão dos testes cerca de 10 a 15 usuários que correspondam a esses perfis. As características destes participantes são descritas a seguir:

> - Pessoas entre 18 e 70 anos;
> - Homens e mulheres de diferentes classes sociais e níveis de familiaridade com tecnologia;
> - Possuem conexão com internet estável;
> - Possuem algum dispositivo eletrônico como computador, tablet ou celular;
> - Possuem e-mail válido para comunicação.

# 4. Procedimento

> - Envio dos links de acesso e orientações sobre acesso ao teste aos participantes;
> - Inicialização das ferramentas para gravação e reunião dos participantes;
> - Recepção dos participantes e esclarecimentos sobre a privacidade de dados (em conformidade com a LGPD), além de aceitação do termo;
> - Orientação sobre o teste: indicação dos objetivos, garantia de anonimato; observação por meio de registro de áudio, vídeo e anotações;
> - Teste: apresentação dos casos de tarefas e suas medições;
> - Debriefing do participante: entrevista após aplicação dos testes e abertura para comentários sobre o produto e preferências.

Os requisitos para realização dos testes são:

> - Conectividade de internet;
> - Navegador: Chrome, Firefox, Safari ou Edge;
> - Disponibilidade do participante em acessar as ferramentas a serem utilizadas no teste (Google Meet, Webcam e Lookback).
 
# 5. Tarefas Testes

Os participantes terão como responsabilidades realizar e analisar as tarefas de modo eficiente, além de comunicar sua opinião sobre a aplicação.

As seguintes tarefas devem ser realizadas pelos participantes:

| Caso de Teste | CTU-01 – Cadastro de Usuário e Aceite de Termos |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Verificar se a tela de cadastro é intuitiva e se o Termo de Responsabilidade é compreendido pelo usuário. |
| Ações necessárias 	| - Acessar o link do site. <br> - Clicar em "Cadastre-se" <br> -Preencher os campos obrigatórios e selecionar o tipo de perfil (Doador ou Beneficiário). <br> - No caso do perfil Doador, marcar o aceite do Termo de Responsabilidade. <br> - Clicar em "Cadastrar" e verificar a mensagem de confirmação. |

| Caso de Teste | CTU-02 – Edição de Perfil e Envio de Documentos |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Verificar a usabilidade no processo de edição de dados e anexo de arquivos de comprovação |
| Ações necessárias 	| - Fazer login com conta previamente cadastrada. <br> - Acessar o perfil através da foto no cabeçalho. <br> - Editar ao menos um campo de informação e salvar. <br> - Anexar documentos de comprovação no campo destinado. <br> - Clicar em "Salvar" e verificar a mensagem de sucesso. |

| Caso de Teste | CTU-03 – Cadastro de Produto para Doação |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	| Verificar o fluxo de inclusão de itens e compreensão dos campos de limite de retirada por classe de beneficiário. |
| Ações necessárias 	| - A partir da homepage, clicar em "Cadastrar produto" <br> - Preencher os dados do alimento (foto, validade, armazenamento) <br> - Delimitar a quantidade por classe de beneficiário nos campos dedicados. <br> - Clicar em "Salvar" e verificar se o produto aparece no histórico. |

| Caso de Teste | CTU-04 – Buscar Produtos na Vitrine e Filtrar |
|:---:	|:---:	|
|	Perfil 	| Beneficiário |
| Objetivo do Teste 	| Analisar a facilidade de busca e filtragem de alimentos disponíveis em tempo real. |
| Ações necessárias 	| - Acessar a página de buscas/vitrine <br> - Utilizar a barra de pesquisa com o nome de um produto <br> - Aplicar filtros disponíveis (distância, categoria, data de validade).<br> - Navegar pela lista de produtos resultantes e verificar se as informações estão claras. |

| Caso de Teste | CTU-05 – Solicitar Reserva de Doação |
|:---:	|:---:	|
|	Perfil 	| Beneficiário |
| Objetivo do Teste 	| Verificar a facilidade do fluxo de solicitação e a clareza da alteração de status do item para "reservado". |
| Ações necessárias 	| - Na vitrine, selecionar um produto disponível. <br> - Clicar em "Solicitar Reserva" <br> - Informar a quantidade desejada no modal <br> - Clicar em "Reservar" e verificar a confirmação e atualização de status. |

| Caso de Teste | CTU-06 – Confirmar Data de Retirada |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	| Analisar a usabilidade do painel de aprovação de reservas recebidas e clareza do fluxo de confirmação. |
| Ações necessárias 	| - Acessar a notificação ou histórico de propostas recebidas. <br> - Clicar em uma solicitação pendente <br> - Clicar em "Confirmar Reserva" <br> - Informar a data e horário disponíveis para retirada e notificar o beneficiário. |

| Caso de Teste | CTU-07 – Validar Entrega com Código/QR Code |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	| Verificar se o fluxo de conclusão da entrega por código numérico ou QR Code é compreensível e rápido. |
| Ações necessárias 	| - Acessar a reserva aprovada <br> - Clicar em "Concluir Doação" <br> - Inserir o código numérico ou escanear o QR Code fornecido pelo beneficiário. <br> - Verificar a mensagem de conclusão e atualização de status. |

| Caso de Teste | CTU-08 – Moderação de Usuários e Documentos |
|:---:	|:---:	|
|	Perfil 	| Administrador |
| Objetivo do Teste 	| Verificar se o painel administrativo é claro e eficiente para análise e aprovação de documentações. |
| Ações necessárias 	| - Logar como Administrador <br> - Acessar "Lista de Usuários" no painel <br> - Clicar em um usuário e baixar o documento pendente. <br> - Aprovar ou rejeitar a documentação (em caso de rejeição, inserir justificativa).<br> - Clicar em "Salvar" e verificar a atualização do status do usuário. |

| Caso de Teste | CTU-09 – Gerar relatório de impacto (CO₂ / Volume) |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	|Verificar se as métricas socioambientais são apresentadas de forma clara e se a exportação é intuitiva |
| Ações necessárias 	| - Acessar "Painel de Impacto Ambiental" ou "Métricas". <br> - Visualizar os gráficos e indicadores exibidos. <br> - Manipular os filtros disponíveis.. <br> - Clicar em "Exportar" e verificar o download do relatório. |

| Caso de Teste | CTU-10 – Visualizar Perfil Público e Avaliar Usuário |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	|Analisar a transparência das informações públicas e a facilidade de uso do sistema de avaliação. |
| Ações necessárias 	| - A partir de uma reserva concluída, clicar no ícone do outro usuário. <br> - Visualizar os dados públicos exibidos no perfil. <br> - Clicar em "Avaliar Experiência".<br> - Inserir uma avaliação de 1 a 5 estrelas e um comentário.<br> - Clicar em "Enviar Avaliação" e verificar se aparece no perfil avaliado. |

| Caso de Teste | CTU-11 – Iniciar Comunicação via Chat |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Validar se o ícone e a interface de chat interno são fáceis de localizar e usar |
| Ações necessárias 	| - Localizar o botão de mensagens dentro de uma reserva/doação ativa <br> - Digitar e enviar uma mensagem para a outra parte. <br> - Verificar visualmente a confirmação de envio. <br> - Confirmar se o destinatário recebe a notificação da mensagem. |

| Caso de Teste | CTU-12 – Recebimento e leitura de notificações |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Verificar se as notificações do sistema são visíveis, compreensíveis e acessíveis ao usuário. |
| Ações necessárias 	| - Realizar o login na aplicação. <br> - Clicar no ícone de notificações no cabeçalho. <br> - Verificar se as notificações de aprovação, recusa ou lembrete estão listadas. <br> - Clicar em uma notificação e verificar se redireciona corretamente para a ação relacionada. |

# 6. Cenários Testes

Os participantes terão como responsabilidades simular e analisar eficientemente os cenários descritos, expondo sua opinião sobre a aplicação.

| Cenário de Teste | CTU-13 – Doando produtos excedentes de casa |
|:---:	|:---:	|
|	Perfil 	| Cidadão Consciente (Doador PF) |
| Objetivo do Teste 	| Verificar a fluidez do fluxo completo desde o cadastro até a publicação do produto na vitrine.|
| Cenário	| Como cidadão focado em sustentabilidade, você comprou alimentos a mais e percebeu que vão vencer. Acesse a plataforma, cadastre-se como Doador PF, aceite os Termos de Responsabilidade e cadastre esses itens na vitrine, incluindo fotos e especificando quantas unidades cada tipo de beneficiário pode retirar. |

| Cenário de Teste | CTU-14 – ONG buscando mantimentos |
|:---:	|:---:	|
|	Perfil 	| Gestora Social (Receptora PJ) |
| Objetivo do Teste 	| Avaliar a usabilidade da vitrine, dos filtros e do fluxo de solicitação de reserva. |
| Cenário 	| Como coordenadora de uma ONG, você precisa encontrar alimentos urgentes para as refeições de amanhã. Faça login, busque produtos na vitrine usando filtros de localização e categoria, encontre alimentos de um comerciante local e solicite a reserva informando a quantidade necessária para sua instituição. |

| Cenário de Teste | CTU-15 – Comerciante organizando a entrega e impacto |
|:---:	|:---:	|
|	Perfil 	| Comerciante (Doador PJ) |
| Objetivo do Teste 	| Verificar a clareza do fluxo de notificação de reserva, validação por código e geração de relatório.|
| Cenário 	| Como dono de comércio, você recebeu uma notificação de que uma ONG deseja seus alimentos. Acesse o sistema e confirme a data de retirada. Simule a chegada da ONG: valide a entrega inserindo o código fornecido por eles. Em seguida, acesse o Painel de Impacto para exportar o relatório de CO₂ evitado neste mês |

| Cenário de Teste | CTU-16 – Chefe de família buscando ajuda alimentar |
|:---:	|:---:	|
|	Perfil 	| Chefe de Família (Receptor PF) |
| Objetivo do Teste 	| Analisar a acessibilidade mobile e a clareza da interface para usuários com menor letramento digital |
| Cenário 	| Usando seu celular com internet limitada, você precisa verificar o status de uma reserva feita ontem. Faça login, acesse a área de notificações e verifique se sua reserva foi aprovada pelo doador. Ao confirmar a aprovação, localize e anote o código gerado para apresentar no momento da retirada |

| Cenário de Teste | CTU-17 – Auditoria e segurança da plataforma |
|:---:	|:---:	|
|	Perfil 	| Administrador |
| Objetivo do Teste 	| Verificar a usabilidade do painel de moderação e das métricas globais da plataforma.|
| Cenário 	| Como administrador, você precisa garantir que fraudes não ocorram. Acesse o painel, visualize a lista de novos usuários, analise o documento enviado por uma nova ONG e aprove o cadastro. Em seguida, acesse a área de métricas globais e visualize o relatório de impacto da plataforma. |

| Cenário de Teste | CTU-18 – Processo de conclusão da doação e feedback final |
|:---:	|:---:	|
|	Perfil 	| Beneficiário |
| Objetivo do Teste 	| Simular o fluxo completo de comunicação interna e uso do sistema de avaliação após a conclusão da doação. |
| Cenário 	| Você reservou um item, mas precisa avisar ao doador que chegará 30 minutos mais tarde. Use o chat interno para este alinhamento. Após simular que a retirada ocorreu, deixe uma avaliação de 5 estrelas com um comentário positivo no perfil do doador. |

| Cenário de Teste | CTU-19 – Primeiro acesso de usuário com baixo letramento digital |
|:---:	|:---:	|
|	Perfil 	| Receptor PF (Chefe de Família em vulnerabilidade) |
| Objetivo do Teste 	| Avaliar se a interface é acessível e compreensível para usuários com pouca familiaridade com tecnologia, sem assistência. |
| Cenário 	| Você nunca usou um aplicativo de doações e seu celular é básico. Sem ajuda do moderador, tente criar uma conta, encontrar um alimento na vitrine e fazer uma solicitação de reserva. O moderador observará onde surgem dúvidas ou bloqueios sem intervir, registrando os pontos de fricção. |

| Cenário de Teste | CTU-20 – Recuperação de senha e reacesso |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Verificar se o fluxo de recuperação de senha é claro e executável de forma autônoma pelo usuário. |
| Cenário 	| Você esqueceu sua senha e precisa entrar na plataforma para verificar uma doação urgente. A partir da tela de login, tente recuperar o acesso usando apenas o e-mail cadastrado, siga as instruções recebidas e cadastre uma nova senha. O moderador registrará dificuldades ou abandono do processo. |

# 7. Análise dos dados

Várias métricas podem ser estabelecidas para análise dos dados coletados. A eficácia da aplicação pode ser mensurada pela quantidade de conclusão de tarefas sem erro, com erro não crítico, erros críticos, quantidade de ações utilizadas e solicitações de assistência. A eficiência pode ser medida pelo tempo de execução da tarefa. 
A satisfação pode ser analisada pelas reações percebidas durante e após a execução das tarefas.

A tabela a seguir será utilizada para registrar os dados de cada tarefa e cenário executados por cada participante:

| **Usuário**   | **Resposta emocional**   | **Execução**  | **Tempo (seg)**  |  **Ações/Cliques**  | **Cometeu erro?** | **Se recuperou do erro?**  | **Observações** | 
| :--------: | :--------: |  :--------: |  :--------: | :--------: | :--------: | :--------: | :--------: |
| Usuario01 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario02 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario03 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario04 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario05 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario06 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario07 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario08 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario09 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario10 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |

As seguintes informações serão utilizadas para preencher a tabela:

> - Usuário: código de cada usuário;
> - Resposta emocional: confuso, confiante, neutro, satisfeito, insatisfeito, ou estressado;
> - Execução: concluiu ou não concluiu;
> - Tempo (seg): cronometrar e colocar o tempo em segundos utilizado para realização ou não da tarefa;
> - Ações/Cliques: quantidade de movimentos e cliques foram feitos para realização ou não da tarefa;
> - Cometeu erro?: sim ou não;
> - Se recuperou do erro?: sim, não ou n/a.
