using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class ServicoConversacaotHub : Hub
    {
        private readonly string _servidor;
        private readonly IDictionary<string, conexaoUsuario> _conexoes;

        public ServicoConversacaotHub(IDictionary<string, conexaoUsuario> conexoes)
        {
            _servidor = "Servidor";
            _conexoes = conexoes;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_conexoes.TryGetValue(Context.ConnectionId, out conexaoUsuario conexaoUsuario))
            {
                _conexoes.Remove(Context.ConnectionId);
                Clients.Group(conexaoUsuario.Sala).SendAsync("RecebendoMensagem", _servidor, $"{conexaoUsuario.Usuario} se desconectou");
                MandeTodosUsuarios(conexaoUsuario.Sala);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task EntrarNaSala(conexaoUsuario conexaoUsuario)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conexaoUsuario.Sala);

            _conexoes[Context.ConnectionId] = conexaoUsuario;

            await Clients.Group(conexaoUsuario.Sala).SendAsync("RecebendoMensagem", _servidor, $"{conexaoUsuario.Usuario} se juntou a sala {conexaoUsuario.Sala}");

            await MandeTodosUsuarios(conexaoUsuario.Sala);
        }

        public async Task EnviarMensagem(string mensagem)
        {
            if (_conexoes.TryGetValue(Context.ConnectionId, out conexaoUsuario userConnection))
            {
                await Clients.Group(userConnection.Sala).SendAsync("RecebendoMensagem", userConnection.Usuario, mensagem);
            }
        }

        public Task MandeTodosUsuarios(string sala)
        {
            var usuarios = _conexoes.Values
                .Where(c => c.Sala == sala)
                .Select(c => c.Usuario);

            return Clients.Group(sala).SendAsync("UsuariosNaSala", usuarios);
        }
    }
}