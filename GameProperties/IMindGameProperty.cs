using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindGame.GameProperties
{
    public interface IMindGameProperty
    {
        /// <summary>
        /// Identifier for data storage
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Localized label
        /// </summary>
        string Label { get; }
        /// <summary>
        /// Gets ids from a game
        /// <param name="game"></param>
        /// </summary>
        List<Guid> GetIds(Game game);
        /// <summary>
        /// Gets a name of a value.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetValue(Guid id);
        /// <summary>
        /// Returns true, if this property is allowed according to settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        bool IsAllowed(MindGameSettings settings);

    }
}
