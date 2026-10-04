using System.Reflection;

namespace TaskFlow.Tests.TestSupport
{
    /// <summary>
    /// Utilitários para testes de domínio. As entidades possuem Id com setter privado
    /// (preenchido pelo EF Core), então usamos reflection para simular a persistência.
    /// </summary>
    public static class EntityTestExtensions
    {
        public static T ComId<T>(this T entidade, int id) where T : class
        {
            var propriedade = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException(
                    $"A entidade {typeof(T).Name} não possui uma propriedade pública 'Id'.");

            propriedade.SetValue(entidade, id);
            return entidade;
        }
    }
}
