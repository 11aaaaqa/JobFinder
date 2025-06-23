Проект реализуется на микросервисной архитектуре. В настоящее время работа над проектом продолжается, поэтому используется стандартный макет (Layout) из шаблона, который в дальнейшем будет доработан и изменён. Некоторые страницы на данный момент ещё не стилизованы.
Проект включает в себя процесс регистрации пользователей в качестве работника или работодателя на основе JWT-токенов, а также подтверждение электронной почты посредством отправки соответствующего письма.
![image](https://github.com/user-attachments/assets/72fada6d-fc03-4135-ab75-0ae98e227a98)
![image](https://github.com/user-attachments/assets/3b23ce57-bf24-40aa-9eac-318f2491d371)

Работодатель может создавать компании, приглашать пользователей, зарегистрировавшихся как работодатели, принимать или отклонять их запросы на вступление, а также просматривать список коллег. Владелец компании по умолчанию обладает полными правами и может выполнять все действия от имени компании, такие как создание, удаление, архивация вакансий, обновление информации о компании. Для остальных участников, входящих в состав компании и не являющихся её владельцами, выполнение данных действий требует наличия соответствующих разрешений, которые выдаются владельцем компании или доверенными пользователями.
![image](https://github.com/user-attachments/assets/093b08df-0648-4652-8397-74a98336500b)

Работодатель может создавать, редактировать, удалять, архивировать, разархивировать вакансии, просматривать список активных и архивных вакансий своей компании
![image](https://github.com/user-attachments/assets/1fe1b4af-8996-4f15-aeec-d18173d30704)
![image](https://github.com/user-attachments/assets/9edfb963-c364-47cc-a450-67855d74d38e)
![image](https://github.com/user-attachments/assets/d6527a60-4351-4c9a-affc-4b0359bcc62b)
![image](https://github.com/user-attachments/assets/3930f5cd-9e6d-408b-8b42-2fdf2d9fdd64)
![image](https://github.com/user-attachments/assets/064d0da8-65a8-442e-83bc-8eb016c116ab)
![image](https://github.com/user-attachments/assets/9ca21ad9-40dd-4f69-becb-08ba7ee72af9)
![image](https://github.com/user-attachments/assets/b7ada3dc-024b-447e-b0e5-30784d7ee807)
![image](https://github.com/user-attachments/assets/23fbc1b9-fee8-4a1c-8e7c-cce052b8237c)
<br/><br/>
Работник может создавать, редактировать резюме, обновлять общую для всех своих резюме информацию
![image](https://github.com/user-attachments/assets/3b971cbc-e605-457c-81e8-09b22931ba65)
![image](https://github.com/user-attachments/assets/9cd656fb-acef-4099-97ad-e2d20cee8647)
![image](https://github.com/user-attachments/assets/20e05081-5096-4e15-b306-a9c151a5e3fc)
![image](https://github.com/user-attachments/assets/e751077a-5f91-4ae4-be41-e821545f6cee)
![image](https://github.com/user-attachments/assets/dd34b55f-9b17-4d97-bd76-431d69573b0f)
<br/>
Просмотр своих резюме:
![image](https://github.com/user-attachments/assets/a0026c9f-54a2-418e-aef1-9ced6b8ef850)
![image](https://github.com/user-attachments/assets/a245d33e-eb0d-4b92-97fc-b58691278eec)
![image](https://github.com/user-attachments/assets/79243801-36bf-4621-a26f-1b98a9ebb011)
![image](https://github.com/user-attachments/assets/40706348-6ab1-4efc-b7d7-7cac7676c601)
![image](https://github.com/user-attachments/assets/f4ce29c9-e07e-42f3-9fe6-80d8952d2b41)
<br/>
Также работник может менять статус, согласно которому его резюме будет или не будет отображаться в списке у работодателя
![image](https://github.com/user-attachments/assets/7c24ff1e-042d-4acb-ac3d-248111d74fd9)
<br/>
Работник может откликаться на вакансию, добавлять её в избранное
![image](https://github.com/user-attachments/assets/73580b6a-7540-4106-9a84-8c67b3b09c46)
<br/>
Если у работника больше одного резюме, то можно выбрать какое отправить
![image](https://github.com/user-attachments/assets/81ddca03-f42e-468b-8d1d-300a721a24d5)
<br/>
Мониторинг своих откликов и их статусов:
![image](https://github.com/user-attachments/assets/9e5acacf-b0eb-4956-8d63-c2f4fde8d0b2)
<br/>
Работодатель может просматривать отклики на все вакансии, сортировать их, проводить в них поиск, а также просматривать списки откликов по отдельной вакансии.
После принятия отклика, создаётся чат работника и работодателя
![image](https://github.com/user-attachments/assets/24f3e519-b803-4f94-b608-294253d1da2e)
<br/>
Если работодатель без отклика работника решит пригласить его на собеседование, то также создастся чат и автоматически отправится шаблонное сообщение. Если же чат уже существует, то отправка будет осуществлена в него
![image](https://github.com/user-attachments/assets/859f0288-f706-4b10-809f-b64536d8dac9)
<br/>
Для создания чата необязательно приглашать на собеседование. В каждом резюме работодателю видна кнопка, с помощью которой можно создать чат или перейти в уже существующий с этим работником
![image](https://github.com/user-attachments/assets/20f53356-bd90-4de8-ac84-d0216d610474)
![image](https://github.com/user-attachments/assets/35e61f49-d0f9-4d4a-b522-ea4f2c2b420f)
<br/>
Также можно просматривать список созданных чатов
![image](https://github.com/user-attachments/assets/26335c50-7dcc-4fff-8a66-3eec12e9cbdd)
