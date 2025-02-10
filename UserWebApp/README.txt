Пишем код: UsersService, Web.config

Публикация IIS:

	Visual Studio:
	"Folder"
	Укажите локальный путь для публикации (например: bin\Release\Publish)

	Настройка IIS:
	Откройте IIS Manager
	Правой кнопкой по "Sites" → "Add Website"
	Заполните:
	Site name: YourAppName
	Physical path: путь к опубликованным файлам
	Binding:
	Type: http/https
	IP address: выбрать IP адрес пк
	Port: 80 (или выбранный порт)
	Host name: пусто

	Настройка прав доступа:
	Правой кнопкой по папке приложения → Properties
	Security → Edit → Add
	Введите: IIS_IUSRS
	Дайте права: Read & Execute, List folder contents, Read

Вести в cmd
%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -i

Откройте Панель управления → Программы → Включение или отключение компонентов Windows
Разверните узлы:
Internet Information Services
Службы World Wide Web
Средства разработки приложений

Убедитесь, что установлены:
[✓] ASP.NET 4.8
[✓] Расширяемость .NET 4.8
[✓] Фильтры ISAPI
[✓] Расширения ISAPI

Выполните в командной строке (от имени администратора):
%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -i
После выполнения должно появиться сообщение:
Finished installing ASP.NET (4.0.30319.0)

Убедитесь в наличии записи для .asmx:
Extension: .asmx
Path: %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll
Verbs: GET,POST,DEBUG

Если записи нет, добавьте вручную:
Нажмите Добавить модульное сопоставление
Запрос: *.asmx
Модуль: IsapiModule
Исполняемый файл: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll
Имя: ASMX-ISAPI-4.0_64bit

В IIS Manager выберите сайт → MIME-типы
Убедитесь в наличии:
Extension: .asmx
MIME type: application/soap+xml

Если отсутствует, добавьте вручную:
Новое расширение: .asmx
Тип MIME: application/soap+xml

Пул приложений: должен использовать .NET 4.0
Режим управляемого конвейера: Integrated

Откройте Панель управления → Брандмауэр Защитника Windows → Дополнительные параметры.
Создайте новое правило для входящих подключений:
Тип: Порт
Протокол: TCP
Укажите порт: 85
Разрешить подключение.

http://192.168.1.100:85/UserService.asmx