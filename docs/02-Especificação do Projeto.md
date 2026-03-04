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


Identifique, em torno de, 5 personas. Para cada persona, lembre-se de descrever suas angústicas, frustrações e expectativas de vida relacionadas ao problema. Além disso, defina uma "aparência" para a persona. Para isso, você poderá utilizar sites como [https://this-person-does-not-exist.com/pt#google_vignette](https://this-person-does-not-exist.com/pt) ou https://thispersondoesnotexist.com/ 

Utilize também como referência o exemplo abaixo:

<img src="https://github.com/ICEI-PUC-Minas-PMV-ADS/IntApplicationProject-Template/blob/main/docs/img/AnaClara1.png" alt="Persona1"/>

Enumere e detalhe as personas da sua solução. Para tanto, baseie-se tanto nos documentos disponibilizados na disciplina e/ou nos seguintes links:

> **Links Úteis**:
> 
> - [Rock Content](https://rockcontent.com/blog/personas/)
> - [Hotmart](https://blog.hotmart.com/pt-br/como-criar-persona-negocio/)
> - [O que é persona?](https://resultadosdigitais.com.br/blog/persona-o-que-e/)
> - [Persona x Público-alvo](https://flammo.com.br/blog/persona-e-publico-alvo-qual-a-diferenca/)
> - [Mapa de Empatia](https://resultadosdigitais.com.br/blog/mapa-da-empatia/)
> - [Mapa de Stalkeholders](https://www.racecomunicacao.com.br/blog/como-fazer-o-mapeamento-de-stakeholders/)
>
Lembre-se que você deve ser enumerar e descrever precisamente e personalizada todos os clientes ideais que sua solução almeja.

## Histórias de Usuários

Com base na análise das personas forma identificadas as seguintes histórias de usuários:

|EU COMO... `PERSONA`| QUERO/PRECISO ... `FUNCIONALIDADE` |PARA ... `MOTIVO/VALOR`                 |
|--------------------|------------------------------------|----------------------------------------|
|Doador/ Beneficiário  |Deseja se cadastrar na plataforma por meio de inclusão de dados/informações próprias.   | Para ter acesso a plataforma como doador ou receptor das mercadorias.               |
|Doador      | Deseja cadastrar doações em lote através da importação de uma planilha.                | Para otimizar o tempo da equipe, cadastrando de um item a centenas de itens próximos ao vencimento de uma única vez. |
|Doador       | Precisa cadastrar doações individualmente, informando foto, validade, tipo de armazenamento e demais classificações.                 | Para conseguir publicar excedentes pontuais de forma rápida e direta na vitrine. |
|Doador       | Necessita validar a doação com um código ou QR Code apresentado pelo beneficiário no momento da entrega.                 | Para dar baixa no sistema, confirmar a entrega de forma segura e manter a rastreabilidade da doação. |
|Doador    | Necessita confirmar ou recusar a reserva solicitada pelo receptor.                 | Para que possa confirmar a disponibilidade ou preparar a logistica de entrega. |
|Doador       | Deseja visualizar um painel com métricas do seu impacto (como kg doados e CO₂ evitado).                | Para ter controle da sua ação social e comprovar a redução de resíduos orgânicos para relatórios de sustentabilidade. |
|Doador      | Desejo delimitar a quantidade que cada beneficiario pode retirar de determinado produto.                 | Para que não sejam realizados pedidos de forma abusiva pelo beneficiario. |
|Beneficiário       | Deseja visualizar quantidade de mercadorias recebidas pelos doadores.                 | Para ter o controle da quantidade de alimentos que recebeu. |
|Beneficiário       | Deseja visualizar uma vitrine de doações e filtrá-las por distância e categoria.                 | Para encontrar de forma rápida os alimentos disponíveis mais próximos à sua localização atual. |
|Beneficiário       | Precisa fazer a reserva de uma doação específica listada no site.                 | Para ter a garantia de que o alimento estará disponível ao chegar no local de retirada, evitando viagens perdidas. |
|Doador/ Beneficiário      | Necessita receber notificações (e-mail ou mensagem) sobre o status de suas reservas.                | Para ser notificado qual o status de como está a reserva ou doação. |
|Doador/ Beneficiário      | Precisa avaliar a experiência com o doador ou beneficiario.                 | Para indicar aos demais usuários a experiencia com da entrega ou recebimento das doações. |
|Administrador       | Deseja validar os documentos enviados pelas ONGs e instituições no momento do cadastro.                 | Para garantir a integridade da plataforma, assegurando que os beneficiários sejam entidades reais e confiáveis. |
|Administrador       | Deseja extrair os dados gerados pelas doações realizadas pela plataforma.                | Para validar o impacto gerado pelo sistema na sociedade. |
|Doador/Beneficiário/Administrador       | Desejo visualizar o perfil publico dos demais usuários.                | Para visualizar as informações publicas e a media de avaliações do perfil. |

## Requisitos

As tabelas que se seguem apresentam os requisitos funcionais e não funcionais que detalham o escopo do projeto.

### Requisitos Funcionais

|ID    | Descrição do Requisito  | Prioridade |
|------|-----------------------------------------|----|
|RF-001| A aplicação deve permitir que o usuário avalie uma agência de intercâmbio com base na sua experiência| ALTA | 
|RF-002| A aplicação deve permitir que o usuário inclua comentários ao fazer uma avaliação de uma agência de intercâmbio    | ALTA |
|RF-003| A aplicação deve permitir que o usuário consulte todas as agências de intercâmbio cadastradas ordenando-as com base em suas notas | ALTA |

### Requisitos não Funcionais

|ID     | Descrição do Requisito  |Prioridade |
|-------|-------------------------|----|
|RNF-001| A aplicação deve ser responsiva | MÉDIA | 
|RNF-002| A aplicação deve processar requisições do usuário em no máximo 3s |  BAIXA | 

Com base nas Histórias de Usuário, enumere os requisitos da sua solução. Classifique esses requisitos em dois grupos:

- [Requisitos Funcionais
 (RF)](https://pt.wikipedia.org/wiki/Requisito_funcional):
 correspondem a uma funcionalidade que deve estar presente na
  plataforma (ex: cadastro de usuário).
- [Requisitos Não Funcionais
  (RNF)](https://pt.wikipedia.org/wiki/Requisito_n%C3%A3o_funcional):
  correspondem a uma característica técnica, seja de usabilidade,
  desempenho, confiabilidade, segurança ou outro (ex: suporte a
  dispositivos iOS e Android).
Lembre-se que cada requisito deve corresponder à uma e somente uma
característica alvo da sua solução. Além disso, certifique-se de que
todos os aspectos capturados nas Histórias de Usuário foram cobertos.

## Restrições

O projeto está restrito pelos itens apresentados na tabela a seguir.

|ID| Restrição                                             |
|--|-------------------------------------------------------|
|01| O projeto deverá ser entregue até o final do semestre |
|02| Para o desenvolvimento do Back-end deverá ser utilizado C#.   |


Enumere as restrições à sua solução. Lembre-se de que as restrições geralmente limitam a solução candidata.

> **Links Úteis**:
> - [O que são Requisitos Funcionais e Requisitos Não Funcionais?](https://codificar.com.br/requisitos-funcionais-nao-funcionais/)
> - [O que são requisitos funcionais e requisitos não funcionais?](https://analisederequisitos.com.br/requisitos-funcionais-e-requisitos-nao-funcionais-o-que-sao/)

## Diagrama de Casos de Uso

![Diagrama de Caso de Uso](https://github.com/user-attachments/assets/869a642a-bede-4b18-9a06-69bba586a5ad)
