using Microsoft.AspNetCore.SignalR;
using poker.api.Models;
using poker.api.Services;

namespace poker.api.Hubs;

public class PokerHub(IPokerService pokerService) : Hub
{

    public async Task JoinTable(string tableId, string playerName)
    {
        var table = await pokerService.JoinTable(tableId, Context.ConnectionId, playerName);
        if (table is null)
            throw new HubException(Constants.HubExceptionMessage.NoTableJoin);

        var player = table.Players[Context.ConnectionId];
        await Groups
            .AddToGroupAsync(Context.ConnectionId, player.GroupName);
        await Clients
            .OthersInGroup(player.GroupName)
            .SendAsync(Constants.HubMethods.ReceiveTable);
    }

    public async Task CleanTable(string tableId)
    {
        await pokerService.CleanTable(tableId);
    }
    
    public async Task NotifyNudge(string tableId, string connectionId)
    {
        await Clients.Client(connectionId)
            .SendAsync(Constants.HubMethods.ReceiveNudge);
    }

    public async Task NotifyDeal(string tableId)
    {
        var success = await pokerService.Deal(tableId);
        if (!success)
            throw new HubException(Constants.HubExceptionMessage.NoTableDeal);

        var groupName = pokerService.GroupName(tableId);
        await Clients
            .Group(groupName)
            .SendAsync(Constants.HubMethods.ReceiveDeal);
    }

    public async Task NotifyShow(string tableId, bool show)
    {
        var tableShowValue = await pokerService.SetShow(tableId, show);
        await NotifyTable(tableId, Constants.HubMethods.ReceiveShow, tableShowValue);
    }

    public async Task NotifyIsPlaying(string tableId, bool isPlaying)
    {
        var player = await pokerService.SetIsPlaying(tableId, Context.ConnectionId, isPlaying);
        if (player == null)
            throw new HubException(Constants.HubExceptionMessage.NotInTableYou);

        await NotifyTable(tableId, Constants.HubMethods.ReceivePlayerChange, player);
    }

    public async Task NotifyEstimate(string tableId, Estimate estimate)
    {
        var player = await pokerService.SetEstimate(tableId, Context.ConnectionId, estimate);
        if (player == null)
            throw new HubException(Constants.HubExceptionMessage.NotInTableYou);
        await NotifyTable(tableId, Constants.HubMethods.ReceivePlayerChange, player);
    }

    public async Task KickPlayer(string tableId, string connectionId)
    {
        var player = await pokerService.SetIsPlaying(tableId, connectionId, false);
        if (player == null)
            throw new HubException(Constants.HubExceptionMessage.NotInTableOther);
        await NotifyTable(tableId, Constants.HubMethods.ReceivePlayerChange, player);
    }

    private async Task NotifyTable<T>(string tableId, string method, T data)
    {
        var groupName = pokerService.GroupName(tableId);
        await Clients
            .Group(groupName)
            .SendAsync(method, data);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var table = await pokerService.FindTable(Context.ConnectionId);
        if (table is null)
            return;

        var player = await pokerService.LeaveTable(table.Id, Context.ConnectionId);
        if (player is null)
            return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, player.GroupName);
        await NotifyTable(table.Id, Constants.HubMethods.ReceiveKicked, Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }
}