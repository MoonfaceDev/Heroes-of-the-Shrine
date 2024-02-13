using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class responsible of toggling menus on/off, from pressing ESCAPE and from button clicks
/// </summary>
public class MenuManager : BaseComponent
{
    public GameMenu[] menus;

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    private void HandleEscape()
    {
        foreach (var menu in OpenedMenus)
        {
            menu.Close();
            return;
        }

        OpenMenu(menus[0]); // first menu is the default one, should be "pause" menu
    }

    private IEnumerable<GameMenu> OpenedMenus => menus.Where(menu => menu.Opened);

    public void OpenMenu(GameMenu menu)
    {
        foreach (var currentMenu in OpenedMenus)
        {
            currentMenu.Close();
        }

        menu.Open();
    }
}