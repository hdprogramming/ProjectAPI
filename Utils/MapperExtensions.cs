using System;
using System.Reflection;
namespace ProjectAPI.Utils;
public static class MapperExtensions
{
    
    private static readonly HashSet<string> IngoredNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Password", 
        "PasswordHashed"
        
        // Buraya eklenecek diğer alanlar...
    };
    // Kaynak (source) nesnedeki null olmayan tüm özellikleri hedef (target) nesneye kopyalar.
    public static void MergeNonNullProperties<TSource, TTarget>(this TTarget target, TSource source)
        where TSource : class
        where TTarget : class
    {
        // 1. Kaynak ve Hedef tiplerinin özelliklerini alın.
        Type sourceType = typeof(TSource);
        Type targetType = typeof(TTarget);
        
        // Sadece public instance özelliklerini alıyoruz.
        PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // 2. Döngü ile kaynak özelliklerini kontrol edin.
        foreach (PropertyInfo sourceProp in sourceProperties)
        {
            // Kaynak nesneden özelliğin değerini alın.
            object sourceValue = sourceProp.GetValue(source);

            // Değer null İSE, bu özelliği Yoksay (AutoMapper'daki .Condition(srcMember != null) eşdeğeri)
            if (sourceValue == null)
            {
                continue;
            }

            // 3. Hedef nesnedeki eşleşen özelliği bulun.
            PropertyInfo targetProp = targetType.GetProperty(sourceProp.Name, BindingFlags.Public | BindingFlags.Instance);
            
            // Hedefte bu isimde bir özellik varsa ve yazılabilir (setter'ı) varsa devam edin.
            if (targetProp != null && targetProp.CanWrite)
            {
                // **ÖZEL İŞLEM KONTROLÜ** (Parola gibi özel mantık gerektiren alanlar için)
                // Bu kısım, sizin orijinal sorununuzdaki Parola mantığına karşılık gelir.
                if (IngoredNames.Contains(sourceProp.Name))
                {
                    // Şifre alanlarını yansıma ile kopyalamayın, elle işlenmelidir.
                    // (AutoMapper'daki .Ignore() eşdeğeri)
                    continue; 
                }
                
                // 4. Değeri hedefe atayın.
                targetProp.SetValue(target, sourceValue);
            }
        }
    }
}