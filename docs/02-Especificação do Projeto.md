# Especificações do Projeto

A definição exata do problema e os pontos mais relevantes a serem tratados na plataforma Doalim foram consolidados com a participação ativa de diferentes atores da cadeia de consumo e assistência social. Por meio de pesquisas de campo, entrevistas e análise do cenário atual de insegurança alimentar e desperdício , mapeamos as necessidades de proprietários de comércios locais (padarias, hortifrútis e supermercados) , coordenadores de instituições receptoras (ONGs e cozinhas comunitárias) , além de cidadãos comuns dispostos a doar e famílias em situação de vulnerabilidade.

Para garantir a viabilidade legal e operacional, também foram analisados os parâmetros da Lei nº 14.016/2020 (Lei de Combate ao Desperdício de Alimentos) e as métricas de impacto ambiental. Todos os detalhes, dores e expectativas levantados nesse processo de elicitação foram consolidados na forma de personas e histórias de usuários, orientando o desenvolvimento da aplicação para que ela resolva problemas reais com empatia, segurança e eficiência logística.

## Personas

Ricardo, o Comerciante (Doador PJ)

- Proprietário de uma padaria local de médio porte. Focado na gestão comercial e no fluxo de caixa.

- Quer reduzir perdas operacionais , atuar com responsabilidade social  e ser reconhecido na comunidade.

- Sente-se mal ao descartar pães e doces no fim do dia, mas teme multas ou implicações jurídicas se alguém passar mal com a doação


Marta, a Gestora Social (Receptora PJ)

- Coordenadora de uma ONG que atende 50 famílias. Muito ativa, mas sempre com recursos e tempo escassos.

- Precisa de doações constantes para garantir o fornecimento para as ações assistenciais.

- Perde muito tempo buscando doadores por telefone e tem dificuldade em organizar a logística de coleta no dia a dia.

Carlos, o Administrador (Sistema)

- Analista de sistemas e gestor operacional da plataforma Despensa Solidária.
  
- Perfil técnico e analítico.Garantir a segurança da plataforma e comprovar a efetividade do sistema através da geração de relatórios de impacto ambiental.
  
- Dificuldade em auditar todas as atividades do sistema  e garantir que apenas estabelecimentos e pessoas sérias participem da rede.

Ana, a Cidadã Consciente (Doadora PF)

- Mora em apartamento, trabalha em escritório e odeia o desperdício. Engajada com pautas de sustentabilidade.
  
- Quer destinar corretamente os alimentos que sabe que não vai consumir, contribuindo para diminuir o lixo orgânico.

- Compra itens em excesso que acabam próximos da validade. Quer doar, mas não conhece quem precisa por perto e tem receio de entregar a desconhecidos.

João, o Chefe de Família (Receptor PF)

- Trabalhador informal/desempregado, sustenta 3 filhos. Possui smartphone com hardware modesto e pacote de dados limitado.

- Busca acesso a alimentos em condições adequadas para consumo, com custo zero, garantindo o sustento da família.

- Sente vergonha de pedir auxílio alimentar em locais públicos. Precisa de um aplicativo muito leve, simples de usar e que consuma pouca internet.

## Histórias de Usuários

Com base na análise das personas forma identificadas as seguintes histórias de usuários:

|EU COMO... `PERSONA`| QUERO/PRECISO ... `FUNCIONALIDADE` |PARA ... `MOTIVO/VALOR`                 |
|--------------------|------------------------------------|----------------------------------------|
|Doador/ Beneficiário  | cadastrar na plataforma por meio de inclusão de dados/informações próprias.   | Para ter acesso a plataforma como doador ou beneficiário das mercadorias.               |
|Doador      | cadastrar doações em lote.                | Para otimizar o tempo da equipe no gereciamento de diferentes lotes de um mesmo produto, cada lote com sua data de validade. |
|Doador       | cadastrar doações individualmente, informando foto, validade, tipo de armazenamento e demais classificações.                 | Para conseguir publicar excedentes pontuais de forma rápida e direta na vitrine. |
|Doador       | validar a doação com um código apresentado pelo beneficiário no momento da entrega.                 | Para dar baixa no sistema, confirmar a entrega de forma segura e manter a rastreabilidade da doação. |
|Doador    | confirmar ou recusar a reserva solicitada pelo beneficiário.                 | Para que possa confirmar a disponibilidade ou preparar a logistica de entrega. |
|Doador       | visualizar um painel com métricas do seu impacto (como unidades doadas).                | Para ter controle e comprovar os seus impactos sociais. |
|Doador      | delimitar a quantidade que cada beneficiario pode retirar de determinado produto.                 | Para que não sejam realizados pedidos de forma abusiva pelo beneficiario. |
|Beneficiário       | visualizar quantidade de mercadorias retiradas.                 | Para ter o controle da quantidade de alimentos que recebeu. |
|Beneficiário       | visualizar uma vitrine de doações e filtrá-las por distância e categoria.                 | Para encontrar de forma rápida os alimentos disponíveis mais próximos à sua localização atual. |
|Beneficiário       | fazer a reserva de uma doação específica listada no site.                 | Para ter a garantia de que o alimento estará disponível ao chegar no local de retirada, evitando viagens perdidas. |
|Doador/ Beneficiário      | receber notificações sobre o status de suas reservas.                | Para ser notificado qual o status de como está a reserva ou doação. |
|Doador/ Beneficiário      | avaliar a experiência com o doador ou beneficiario.                 | Para indicar aos demais usuários a experiencia com da entrega ou recebimento das doações. |
|Administrador       | validar os documentos enviados pelas ONGs e instituições no momento do cadastro.                 | Para garantir a integridade da plataforma, assegurando que os beneficiários sejam entidades reais e confiáveis. |
|Administrador       | visualizar relatório de métricas geradas pela plataforma.                | Para validar o impacto gerado pelo sistema na sociedade. |
|Doador/Beneficiário/Administrador       | visualizar o perfil publico dos demais usuários.                | Para visualizar as informações publicas e a media de avaliações do perfil. |

## Requisitos

As tabelas que se seguem apresentam os requisitos funcionais e não funcionais que detalham o escopo do projeto.

### Requisitos Funcionais

|ID    | Descrição do Requisito  | Prioridade |
|------|-----------------------------------------|----|
|RF-001| A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha.| ALTA | 
|RF-002| A aplicação deve exigir o aceite digital obrigatório do "Termo de Responsabilidade" (baseado na Lei 14.016/2020) no momento do cadastro do doador.| ALTA |
|RF-003| A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador.| ALTA |
|RF-004| A aplicação deve permitir que o doador cadastre itens para doação com as informações dos produtos.  | ALTA |
|RF-005| A aplicação deve exibir uma vitrine em tempo real das doações disponíveis, permitindo que o beneficário possa realizar filtros. | ALTA |
|RF-006| A aplicação deve permitir que o beneficiário solicite e reserve uma doação, alterando o status do item no sistema para evitar que outra pessoa reserve o mesmo alimento. | ALTA |
|RF-007| A aplicação deve permitir que o doador valide a entrega da doação através de um código numérico ou QR Code apresentado pelo receptor no momento da retirada. | ALTA |
|RF-008| A aplicação deve possuir um painel administrativo para moderação de conteúdo e gerenciamento de usuários. | ALTA |
|RF-009| A aplicação deve notificar os usuários sobre aprovação ou recusa de reservas, lembretes de retirada e doações expiradas. | MÉDIA |
|RF-010| A aplicação deve gerar relatórios de impacto exibindo o volume total de produtos doados, volume total de beneficiários atendidos, volume total de reservas retiradas, e quantidade de beneficiários e doadores cadastrados na plataforma. | MÉDIA |
|RF-011| A aplicação deve gerar histórico das doações, e relatórios de métricas de doações para o doador, com quantidade de produtos doados, quantidade de reservas retiradas, e para o beneficiário com o status das suas reservas, e no histórico exibe o volume total itens recebidos. | MÉDIA |
|RF-012| A aplicação deve permitir ao doador delimitar no item doado a quantidade que cada classe de beneficiário pode retirar do item. | BAIXA |
|RF-013| A aplicação deve permitir que doador e beneficiário acessem o perfil um do outro para validação dos dados públicos. | BAIXA |
|RF-014| A aplicação deve permitir que doador e beneficiário avaliem um ao outro com 1 a 5 estrelas após a conclusão da retirada. | BAIXA |
|RF-015| A aplicação deve disponibilizar um chat ou sistema de mensagens interno para comunicação direta e alinhamento entre doador e beneficiário. | BAIXA |

### Requisitos não Funcionais

|ID     | Descrição do Requisito  |Prioridade |
|-------|-------------------------|----|
|RNF-001| A aplicação deve conseguir acessar API's | ALTA | 
|RNF-002| A aplicação deve ser desenvolvida utilizando a linguagem C# no back-end e a tríade JavaScript, HTML e CSS (com framework Bootstrap) no front-end. |  ALTA | 
|RNF-003| A aplicação deve estar em total conformidade com a LGPD, garantindo o tratamento seguro de dados sensíveis de famílias cadastradas e políticas claras de privacidade. |  ALTA |
|RNF-004| A aplicação deve possuir uma interface "mobile-first", sendo responsiva e adaptável a telas de celulares, considerando que muitos receptores usarão o sistema via smartphone. |  ALTA |
|RNF-005| A aplicação deve garantir transações no banco de dados e evitar "double booking" (reservas simultâneas da mesma doação). |  ALTA |
|RNF-006| A aplicação deve estar disponível 24 horas por dia, 7 dias por semana. |  ALTA |
|RNF-007| A aplicação deve ser compatível com os principais navegadores (Chrome, Firefox e Edge). |  ALTA |
|RNF-008| A aplicação deve utilizar o Microsoft SQL Server para armazenamento de dados. |  ALTA |
|RNF-009| A aplicação deve ter capacidade de receber multiplos acessos simultaneos. |  ALTA |
|RNF-010| A aplicação deve ter um tempo de resposta inferior a 5 segundos para buscas e listagens de doações na vitrine em condições normais de tráfego. |  MÉDIA |
|RNF-011| A aplicação deve seguir diretrizes básicas de acessibilidade (WCAG), possuindo bom contraste de cores e navegação por teclado. |  MÉDIA |
|RNF-012| A aplicação deve garantir a segurança dos dados utilizando senhas com hash forte e comunicação via protocolo TLS/HTTPS. |  MÉDIA |

## Restrições

O projeto está restrito pelos itens apresentados na tabela a seguir.

|ID| Restrição                                             |
|--|-------------------------------------------------------|
|01| O projeto foi desenvolvido entre fevereiro e junho de 2026, sendo entregue no final do semestre. |
|02| Para o desenvolvimento do Back-end deverá ser utilizado C#.   |

## Diagrama de Casos de Uso

![Diagrama de Caso de Uso](https://github.com/user-attachments/assets/869a642a-bede-4b18-9a06-69bba586a5ad)
