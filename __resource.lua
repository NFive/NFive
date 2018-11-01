-- FiveM resource definition

resource_manifest_version '44febabe-d386-4d18-afbe-5e627f4af937'

server_scripts {
	-- NFive
	'NFive.Server.net.dll',
}

client_scripts {
	-- NFive
	'NFive.Client.net.dll',
}

files {
	-- NFive
	'Newtonsoft.Json.dll',
	'System.ComponentModel.DataAnnotations.dll',
	'NFive.SDK.Core.net.dll',
	'NFive.SDK.Client.net.dll',
	'index.html',
}

ui_page 'index.html'
