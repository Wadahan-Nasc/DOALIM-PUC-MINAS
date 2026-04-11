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
| Objetivo do Teste 	| Verificar a tela de cadastro e a clareza do Termo de Responsabilidade |
| Ações necessárias 	| - Acessar o link do site <br> - Clicar em "Cadastre-se" <br> - Preencher os campos obrigatórios e selecionar o tipo de perfil <br> - Marcar o aceite dos Termos de Responsabilidade <br> - Clicar em "Cadastrar" |

| Caso de Teste | CTU-02 – Edição de Perfil e Envio de Documentos |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Verificar a usabilidade no anexo de arquivos de comprovação |
| Ações necessárias 	| - Fazer login <br> - Acessar o perfil no cabeçalho <br> - Anexar documentos no campo destinado <br> - Clicar em "Salvar" e visualizar a mensagem de sucesso |

| Caso de Teste | CTU-03 – Cadastro de Produto para Doação |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	| Verificar o fluxo de inclusão de itens e limites de retirada |
| Ações necessárias 	| - A partir da homepage, clicar em "Cadastrar produto" <br> - Preencher os dados do alimento (foto, validade, armazenamento) <br> - Delimitar a quantidade por classe de beneficiário <br> - Clicar em "Salvar" |

| Caso de Teste | CTU-04 – Buscar Produtos na Vitrine e Filtrar |
|:---:	|:---:	|
|	Perfil 	| Beneficiário |
| Objetivo do Teste 	| Analisar a busca de alimentos disponíveis em tempo real |
| Ações necessárias 	| - Acessar a página de buscas/vitrine <br> - Utilizar os filtros (distância, categoria) ou barra de pesquisa <br> - Navegar pela lista de produtos resultantes |

| Caso de Teste | CTU-05 – Solicitar Reserva de Doação |
|:---:	|:---:	|
|	Perfil 	| Beneficiário |
| Objetivo do Teste 	| Verificar facilidade na alteração de status do item para "reservado" |
| Ações necessárias 	| - Na vitrine, selecionar um produto <br> - Clicar em "Solicitar Reserva" <br> - Informar a quantidade desejada <br> - Clicar em "Reservar" |

| Caso de Teste | CTU-06 – Confirmar Data de Retirada |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	| Analisar o painel de aprovação de reservas recebidas |
| Ações necessárias 	| - Acessar a notificação ou histórico de propostas <br> - Clicar em uma solicitação pendente <br> - Clicar em "Confirmar Reserva" <br> - Informar data/horário e notificar beneficiário |

| Caso de Teste | CTU-07 – Validar Entrega com Código/QR Code |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	| Verificar a conclusão da entrega da doação |
| Ações necessárias 	| - Acessar a reserva aprovada <br> - Clicar em "Concluir Doação" <br> - Inserir o código numérico fornecido pelo beneficiário <br> - Finalizar operação |

| Caso de Teste | CTU-08 – Moderação de Usuários e Documentos |
|:---:	|:---:	|
|	Perfil 	| Administrador |
| Objetivo do Teste 	| Verificar o painel administrativo |
| Ações necessárias 	| - Logar como Administrador <br> - Acessar "Lista de Usuários" <br> - Baixar e analisar documento pendente <br> - Aprovar ou rejeitar (com justificativa) e salvar |

| Caso de Teste | CTU-09 – Gerar Relatório de Impacto (CO2 / Volume) |
|:---:	|:---:	|
|	Perfil 	| Doador |
| Objetivo do Teste 	| Verificar a clareza das métricas socioambientais |
| Ações necessárias 	| - Acessar "Painel de Impacto Ambiental" ou "Métricas" <br> - Visualizar gráficos <br> - Clicar em "Exportar" para baixar relatório |

| Caso de Teste | CTU-10 – Visualizar Perfil Público e Avaliar Usuário |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Analisar a transparência e sistema de feedback |
| Ações necessárias 	| - Através de uma reserva concluída, clicar no ícone do outro usuário <br> - Visualizar dados públicos <br> - Inserir avaliação (estrelas e comentário) |

| Caso de Teste | CTU-11 – Iniciar Comunicação via Chat |
|:---:	|:---:	|
|	Perfil 	| Doador / Beneficiário |
| Objetivo do Teste 	| Validar se o ícone e a interface de chat interno são fáceis de localizar e usar |
| Ações necessárias 	| - Localizar o botão de mensagens dentro de uma reserva/doação ativa <br> - Enviar uma dúvida/mensagem para a outra parte <br> - Identificar visualmente se a mensagem foi enviada com sucesso |

# 6. Cenários Testes

Os participantes terão como responsabilidades simular e analisar eficientemente os cenários descritos, expondo sua opinião sobre a aplicação.

Os cenários a seguir devem ser realizados pelos participantes:
| Cenário de Teste | CTU-13 – Doando produtos excedentes de casa |
|:---:	|:---:	|
|	Perfil 	| Cidadão Consciente (Doador PF) |
| Objetivo do Teste 	| Verificar a fluidez desde o cadastro até a postagem do produto. |
| Cenário	| Como cidadão focado em sustentabilidade, você comprou alimentos a mais e percebeu que vão vencer. Você precisa acessar a plataforma, se cadastrar como Doador PF, aceitar os termos de responsabilidade e cadastrar esses itens na vitrine, incluindo fotos e especificando quantas unidades cada pessoa pode pegar. |

| Cenário de Teste | CTU-14 – ONG buscando mantimentos |
|:---:	|:---:	|
|	Perfil 	| Gestora Social (Receptora PJ) |
| Objetivo do Teste 	| Avaliar vitrine, filtros e solicitação de reserva. |
| Cenário 	| Como coordenadora de uma ONG, você precisa encontrar alimentos urgentes para as refeições de amanhã. Você deve logar, buscar produtos disponíveis na vitrine usando filtros, encontrar alimentos doados por um comerciante local e solicitar a reserva informando a quantidade necessária para sua instituição. |

| Cenário de Teste | CTU-15 – Comerciante organizando a entrega e impacto |
|:---:	|:---:	|
|	Perfil 	| Comerciante (Doador PJ) |
| Objetivo do Teste 	| Verificar a notificação de reserva, validação por código e relatório. |
| Cenário 	| Como dono de comércio, você recebeu uma notificação de que uma ONG deseja seus alimentos. Você deve acessar o sistema, confirmar a data de retirada. Simule que a ONG chegou: valide a entrega inserindo o código fornecido por eles. Por fim, acesse o painel de impacto para exportar seu relatório de CO₂ evitado neste mês. |

| Cenário de Teste | CTU-16 – Chefe de família buscando ajuda alimentar |
|:---:	|:---:	|
|	Perfil 	| Chefe de Família (Receptor PF) |
| Objetivo do Teste 	| Analisar a acessibilidade móvel e clareza da interface. |
| Cenário 	| Pelo seu celular, com internet limitada, você precisa achar doações. Faça o login, verifique o status da reserva que você fez ontem (se foi aprovada pelo doador) na área de notificações. Ao ver que foi aprovada, anote o código gerado para levar no momento da retirada. |

| Cenário de Teste | CTU-17 – Auditoria e segurança da plataforma |
|:---:	|:---:	|
|	Perfil 	| Administrador |
| Objetivo do Teste 	| Verificar o painel de moderação. |
| Cenário 	| Como administrador, você precisa garantir que fraudes não ocorram. Entre no painel, visualize a lista de novos usuários, analise o documento enviado por uma nova ONG e aprove o cadastro. Depois, vá na área de métricas globais e visualize o relatório de impacto. |

| Cenário de Teste | CTU-18 – Processo de conclusão da doação e feedback final |
|:---:	|:---:	|
|	Perfil 	| Beneficiário |
| Objetivo do Teste 	| Simular o fluxo completo de comunicação interna e sistema de avaliação. |
| Cenário 	| Você reservou um item, mas precisa perguntar ao doador se pode retirar 30 minutos mais tarde. Use o chat interno para este alinhamento. Após simular que a retirada ocorreu, você deseja elogiar a pontualidade do doador. Encontre onde deixar uma avaliação de 5 estrelas e um comentário positivo. |

# 7. Análise dos dados

Várias métricas podem ser estabelecidas para análise dos dados a serem coletados dos testes aplicados.
A eficácia da aplicação pode ser mensurada pela quantidade de conclusão de tarefas sem erro, conclusão de tarefa com erro (não crítico), erros críticos, quantidade de ações utilizadas e quantidade de solicitações de assistência.
A eficiência pode ser medida através do tempo de execução da tarefa e o tempo utilizado nas tentativas de execução da tarefa.
A satisfação pode ser analisada através das reações percebidas diante da execução das tarefas pelos usuários e de forma geral durante o teste.

A tabela a seguir será utilizada para analisar cada tarefa e cenário executados pelos participantes:

| **Usuário**   | **Resposta emocional**   | **Execução**  | **Tempo (seg)**  |  **Ações/Cliques**  | **Cometeu erro?** | **Se recuperou do erro?**  | **Observações** | 
| :--------: | :--------: |  :--------: |  :--------: | :--------: | :--------: | :--------: | :--------: |
| Usuario01 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario02 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario03 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario04 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |
| Usuario05 | -------- |  -------- |  -------- | -------- | -------- | -------- | -------- |

As seguintes informações serão utilizadas para preencher a tabela:

> - Usuário: código de cada usuário;
> - Resposta emocional: confuso, confiante, neutro, satisfeito, insatisfeito, ou estressado;
> - Execução: concluiu ou não concluiu;
> - Tempo (seg): cronometrar e colocar o tempo em segundos utilizado para realização ou não da tarefa;
> - Ações: contabilizar quantos movimentos e cliques foram feitos para realização ou não da tarefa;
> - Cometeu erro?: sim ou não;
> - Se recuperou do erro?: sim, não ou n/a.
