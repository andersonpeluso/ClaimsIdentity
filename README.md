# ClaimsIdentity

A diferença entre autenticação e autorização
Em primeiro lugar, devemos esclarecer a diferença entre essas duas facetas dependentes da segurança. A resposta simples é que a autenticação é o processo de determinar quem você é, enquanto a autorização gira em torno do que você tem permissão para fazer , ou seja, permissões. Obviamente, antes de determinar o que um usuário tem permissão para fazer, você precisa saber quem ele é, portanto, quando a autorização for necessária, você deverá primeiro autenticar o usuário de alguma forma.

## Autenticação em ASP.NET Core
As propriedades fundamentais associadas à identidade não mudaram realmente no ASP.NET Core - embora sejam diferentes, devem ser familiares aos desenvolvedores ASP.NET em geral. Por exemplo, no ASP.NET 4.x, há uma propriedade chamada Useron HttpContext, que é do tipo IPrincipal, que representa o usuário atual para uma solicitação. No ASP.NET Core há uma propriedade semelhante chamada User, a diferença é que essa propriedade é do tipo ClaimsPrincipal, que implementa IPrincipal.

A mudança a ser usada ClaimsPrincipaldestaca uma mudança fundamental na maneira como a autenticação funciona no ASP.NET Core em comparação com o ASP.NET 4.x. Anteriormente, a autorização era tipicamente baseada em funções, então um usuário pode pertencer a uma ou mais funções, e diferentes seções do seu aplicativo podem exigir que um usuário tenha uma função específica para acessá-lo. No ASP.NET Core, esse tipo de autorização baseada em função ainda pode ser usado, mas isso é principalmente por motivos de compatibilidade com versões anteriores. O caminho que eles realmente querem que você siga é a autenticação baseada em declarações.

## Autenticação baseada em declarações
O conceito de autenticação baseada em declarações pode ser um pouco confuso quando você o examina pela primeira vez, mas na prática é provavelmente muito semelhante às abordagens que você já está usando. Você pode pensar nas reivindicações como sendo uma declaração ou propriedade de uma identidade específica. Essa declaração consiste em um nome e um valor. Por exemplo, você pode ter uma DateOfBirthreclamação, FirstNamereclamação, EmailAddressreclamação ou IsVIPreclamação. Note-se que estas declarações são sobre o que ou quem a identidade é, não o que eles podem fazer.

A própria identidade representa uma única declaração que pode ter muitas declarações associadas a ela. Por exemplo, considere uma carteira de motorista. Esta é uma identidade única, que contém uma série de reivindicações - FirstName, LastName, DateOfBirth, Addresse que os veículos que estão autorizados a unidade. Seu passaporte seria uma identidade diferente com um conjunto diferente de reivindicações.

Portanto, vamos dar uma olhada nisso no contexto do ASP.NET Core. As identidades no ASP.NET Core são a ClaimsIdentity. Uma versão simplificada da classe pode ter a seguinte aparência (a classe real é muito maior!).

public async Task<IActionResult> Login(string returnUrl = null)
{
    const string Issuer = "https://gov.uk";

    var claims = new List<Claim> {
        new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
        new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
        new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
        new Claim("ChildhoodHero", "Ronnie James Dio", ClaimValueTypes.String)
    };

    var userIdentity = new ClaimsIdentity(claims, "Passport");

    var userPrincipal = new ClaimsPrincipal(userIdentity);

    await HttpContext.Authentication.SignInAsync("Cookie", userPrincipal,
        new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
            IsPersistent = false,
            AllowRefresh = false
        });

    return RedirectToLocal(returnUrl);
}

Eu mostrei as propriedades principais neste esboço, incluindo Claimsque consiste em todas as declarações associadas a uma identidade. Existem vários métodos utilitários para trabalhar com o Claims, dois dos quais mostrei aqui. Eles são úteis quando você chega à autorização e está tentando determinar se uma determinada identidade tem um dado no qual Claimvocê está interessado.

A AuthenticationTypepropriedade é bastante autoexplicativa. Em nosso exemplo prático anterior, pode ser a string Passportou DriversLicense, mas no ASP.NET é mais provável que seja Cookies, Bearerou Googleetc. É simplesmente o método usado para autenticar o usuário e determinar as declarações associadas a uma identidade .

Por fim, a propriedade IsAuthenticatedindica se uma identidade é autenticada ou não. Isso pode parecer redundante - como você pode ter uma identidade com reivindicações quando ela não está autenticada? Um cenário pode ser quando você permite usuários convidados em seu site, por exemplo, em um carrinho de compras. Você ainda tem uma identidade associada ao usuário e essa identidade ainda pode ter declarações associadas a ela, mas eles não serão autenticados. Esta é uma distinção importante a se ter em mente.

Como adjunto a isso, no ASP.NET Core, se você criar um ClaimsIdentitye fornecer um AuthenticationTypeno construtor, IsAuthenticatedsempre será verdadeiro. Portanto, um usuário autenticado deve sempre ter um AuthenticationTypee, ao contrário, você não pode ter um usuário não autenticado que tenha um AuthenticationType.

## Resumo
Descrevi como funciona a autenticação baseada em declarações e como ela se aplica ao ASP.NET Core. No próximo post, examinarei o próximo estágio do processo de autenticação - como o middleware de cookie realmente faz seu login com o principal fornecido. As postagens subsequentes cobrirão como você pode usar vários manipuladores de autenticação, como a autorização funciona e como a identidade do ASP.NET Core une tudo isso.
