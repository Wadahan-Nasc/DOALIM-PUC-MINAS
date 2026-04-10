
# Projeto de Interface

<span style="color:red">Pré-requisitos: <a href="2-Especificação do Projeto.md"> Documentação de Especificação</a></span>

Visão geral da interação do usuário pelas telas do sistema e protótipo interativo das telas com as funcionalidades que fazem parte do sistema (wireframes).

 Apresente as principais interfaces da plataforma. Discuta como ela foi elaborada de forma a atender os requisitos funcionais, não funcionais e histórias de usuário abordados nas <a href="2-Especificação do Projeto.md"> Documentação de Especificação</a>.

## Diagrama de Fluxo

Login/Cadastro
<img width="4363" height="2258" alt="Login_Cadastro" src="https://github.com/user-attachments/assets/296397c8-4f3f-47a6-83db-8ac23254815f" />

Navegação do Doador
<img width="4738" height="2361" alt="Navegação_Doador" src="https://github.com/user-attachments/assets/a30d4118-15de-4182-89c1-cffc4f7177c5" />

Navegação do Beneficiario
![Navegação Beneficiário Doalim](https://github.com/user-attachments/assets/b9c38a10-13ce-49f9-83c7-8576961e52d4)

Navegação do Administrador
<img width="3488" height="1864" alt="Navegação_Administrador" src="https://github.com/user-attachments/assets/4272c3e2-82da-467e-93df-05e1c331ea9a" />

Vitrine/Reserva
![Vitrine - Reserva](https://github.com/user-attachments/assets/149d263a-d3b5-4630-bef3-d809d6265532)

Diagrama de fluxo elaborado na plataforma Lucidchart.

## Wireframes
Link para a aplicação interativa no Marvelapp: https://marvelapp.com/prototype/11h45d0e

(Observação: Por uma limitação do Marvelapp, para que possa visualizar as telas de Admin, Doador e Beneficiário, deverá seguir de acordo com a legenda e imagem abaixo)

1- Doador

2- Administrador

3- Beneficiário

<img width="899" height="640" alt="image" src="https://github.com/user-attachments/assets/44580fe6-918e-48ed-954e-43c873bbb6f7" />


+ RF-001
  - A aplicação deve permitir que usuários do tipo Doador e Beneficiário realizem cadastro, login, logout e recuperação de senha.
+ RF-002
  - A aplicação deve exigir o aceite digital obrigatório do "Termo de Responsabilidade" (baseado na Lei 14.016/2020) no momento do cadastro do doador.

Login/Autenticação
<img width="1074" height="763" alt="image" src="https://github.com/user-attachments/assets/ef29afbd-a98d-4c3d-8274-030f11fbd21e" />

Cadastro de Usuário
<img width="1068" height="759" alt="image" src="https://github.com/user-attachments/assets/baa9a689-f48d-4ed6-9509-4299e39dbf15" />



+ RF-003
  - A aplicação deve permitir que o usuário gerencie seu perfil, incluindo edição de dados e envio de documentos para verificação do administrador.

Perfil (Editar)
<img width="1071" height="759" alt="image" src="https://github.com/user-attachments/assets/440bde3e-afad-4bb8-9262-2f0aff279002" />



+ RF-004
  - A aplicação deve permitir que o doador cadastre itens para doação com as informações dos produtos.
+ RF-012
  - A aplicação deve permitir ao doador delimitar no item doado a quantidade que cada classe de beneficiário pode retirar do item.

Cadastro de Produto
<img width="1068" height="760" alt="image" src="https://github.com/user-attachments/assets/a99e5289-8911-4e2b-b1ab-35645684c40b" />



+ RF-005
  - A aplicação deve exibir uma vitrine em tempo real das doações disponíveis, permitindo que o beneficário possa realizar filtros.
<img width="1076" height="773" alt="image" src="https://github.com/user-attachments/assets/26a77c97-15e1-4035-a527-5bb0ff25edd1" />



+ RF-006
  - A aplicação deve permitir que o beneficiário solicite e reserve uma doação, alterando o status do item no sistema para evitar que outra pessoa reserve o mesmo alimento.
<img width="1077" height="769" alt="image" src="https://github.com/user-attachments/assets/8a1a6cd7-90fa-49a9-9412-e24583fba13b" />



+ RF-007
  - A aplicação deve permitir que o doador valide a entrega da doação através de um código numérico ou QR Code apresentado pelo receptor no momento da retirada.

Histórico de Doações
<img width="1072" height="761" alt="image" src="https://github.com/user-attachments/assets/1e5c7096-ad9b-4d81-bb3f-133b564ec483" />

Detalhes da Doação
<img width="1069" height="754" alt="image" src="https://github.com/user-attachments/assets/39aa8a7f-f974-4008-951a-e5c9ce3acf2b" />



+ RF-008
  - A aplicação deve possuir um painel administrativo para moderação de conteúdo e gerenciamento de usuários.

 Painel do Administrador
<img width="1072" height="781" alt="image" src="https://github.com/user-attachments/assets/6a14f405-32ef-4fe1-aa85-16fb27a2f07e" />

Gerenciamento de usuários
<img width="1072" height="775" alt="image" src="https://github.com/user-attachments/assets/e01c370f-79f8-4b50-aec4-48fc7a61961e" />
<img width="1078" height="766" alt="image" src="https://github.com/user-attachments/assets/af933748-1460-446c-9aa9-d14683850807" />
<img width="1069" height="765" alt="image" src="https://github.com/user-attachments/assets/17bf5bbd-c840-476b-b0e2-4b6004e16075" />






+ RF-009
  - A aplicação deve notificar os usuários sobre aprovação ou recusa de reservas, lembretes de retirada e doações expiradas.



+ RF-010
  - A aplicação deve gerar relatórios de impacto para o doador, exibindo o volume total doado e a redução estimada de CO₂ gerada por evitar o descarte.

HomePage/Dashboard
 <img width="1075" height="757" alt="image" src="https://github.com/user-attachments/assets/fa1f8cc8-0629-4619-b2c1-403ecdd3986e" />

Painel de Impacto Ambiental
<img width="1072" height="730" alt="image" src="https://github.com/user-attachments/assets/e845ccb5-2804-4ff5-9bcb-a31e02b4c250" />



+ RF-011
  - A aplicação deve gerar relatórios de impacto para o beneficiário, exibindo o volume total itens recebidos.
<img width="1078" height="771" alt="image" src="https://github.com/user-attachments/assets/c52f803c-ac92-46c3-906e-6499d9bab8b4" />

Detalhes da reserva recebida
<img width="1071" height="763" alt="image" src="https://github.com/user-attachments/assets/705cfaa5-e208-4704-b943-199640e9ec95" />

Histórico de doações recebidas
<img width="1079" height="769" alt="image" src="https://github.com/user-attachments/assets/9f4f2758-4a76-4163-9b31-b9c4cea513a0" />






+ RF-013
  - A aplicação deve permitir que doador e receptor acessem o perfil um do outro para validação dos dados públicos.
+ RF-014
  - A aplicação deve permitir que doador e receptor avaliem um ao outro com 1 a 5 estrelas após a conclusão da retirada.

Perfil (Público)
<img width="1065" height="758" alt="image" src="https://github.com/user-attachments/assets/570557f2-1fea-4897-ac6c-46a5d1faa0a0" />



+ RF-015
  - A aplicação deve disponibilizar um chat ou sistema de mensagens interno para comunicação direta e alinhamento entre doador e beneficiário.

Chat
<img width="1072" height="758" alt="image" src="https://github.com/user-attachments/assets/f2fb739a-a238-462a-bca6-faeb5fde1102" />

