using System.Text.RegularExpressions;

namespace Angie.Tools;

struct MatchResult {
  public Dictionary<string, string> paramDict;
  public TrieNode? matched;
}

abstract class TrieNode {
  public List<TrieNode> children;

  public Handler? handler;
  public TrieNode() {
    this.children = new List<TrieNode>();
  }
  abstract public MatchResult matchSelf(string word);
  public MatchResult matchChild(string word) {
    var result = new MatchResult();
    result.matched = null;
    foreach (var child in this.children) {
      var matchResult = child.matchSelf(word);
      if (matchResult.matched != null) {
        result.matched = matchResult.matched;
        result.paramDict = matchResult.paramDict;
        break;
      }
    }
    return result;
  }

  public void insert(List<string> pathWords, Handler handler) {
    if (pathWords.Count == 0) {
      this.handler = handler;
    } else {
      TrieNode? matched = this.matchChild(pathWords[0]).matched;
      if (matched == null) {
        matched = TrieNode.create(pathWords[0]);
        this.children.Add(matched);
      }
      matched.insert(pathWords.Skip(1).ToList(), handler);
    }
  }

  static public TrieNode create(string words) {
    if (words.StartsWith(":")) {
      return new ParamNode(words.Substring(1));
    } else if (words.StartsWith("^") && words.EndsWith("$")) {
      return new RegexNode(words.Substring(1, words.Length - 2));
    } else if (words.Any(c => c == '*')) {
      return new WildcardNode(words);
    } else {
      return new WordNode(words);
    }
  }
}
class ParamNode : TrieNode {
  public string paramName;
  public ParamNode(string paramName) {
    this.paramName = paramName;
  }
  public override MatchResult matchSelf(string word) {
    return new MatchResult() {
      matched = this,
      paramDict = new Dictionary<string, string>() { { this.paramName, word } }
    };
  }
}
class WordNode : TrieNode {
  public string word;
  public WordNode(string word) {
    this.word = word;
  }
  public override MatchResult matchSelf(string word) {
    if (this.word.Equals(word)) {
      return new MatchResult { matched = this };
    } else {
      return new MatchResult { matched = null };
    }
  }
}
class WildcardNode : TrieNode {
  private Regex regex;

  public WildcardNode(string word) {
    this.regex = new Regex(word.Replace("*", ".*"));
  }
  public override MatchResult matchSelf(string word) {
    if (this.regex.IsMatch(word)) {
      return new MatchResult { matched = this };
    } else {
      return new MatchResult { matched = null };
    }
  }
}

class RegexNode : TrieNode {
  private Regex regex;
  public RegexNode(string regex) {
    this.regex = new Regex(regex);
  }
  public override MatchResult matchSelf(string word) {
    if (this.regex.IsMatch(word)) {
      return new MatchResult { matched = this };
    } else {
      return new MatchResult { matched = null };
    }
  }
}
class Trie : TrieNode {
  public override MatchResult matchSelf(string word) {
    return new MatchResult();
  }
  public MatchResult matchPath(string path) {
    var words = path.Split('/');
    var paramDict = new Dictionary<string, string>();
    TrieNode matched = this;
    foreach (var word in words) {
      var matchResult = matched.matchChild(word);
      if (matchResult.matched == null) return new MatchResult { matched = null };
      matched = matchResult.matched;
      if (matchResult.paramDict != null) {
        foreach (var param in matchResult.paramDict) {
          paramDict[param.Key] = param.Value;
        }
      }
    }
    return new MatchResult { matched = matched, paramDict = paramDict };
  }
}
