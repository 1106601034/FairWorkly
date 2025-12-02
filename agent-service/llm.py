import logging
import os
from functools import lru_cache
from pathlib import Path

from dotenv import load_dotenv
load_dotenv()

from langchain_openai import ChatOpenAI
from langchain_core.messages import SystemMessage, HumanMessage

llm = ChatOpenAI(
    model=os.getenv("OPENAI_MODEL", "gpt-4o-mini"),
    temperature=float(os.getenv("MODEL_TEMPERATURE", "0")),
)


@lru_cache(maxsize=None)
def load_prompt(prompt_path: Path, fallback: str) -> str:
    try:
        return Path(prompt_path).read_text().strip()
    except FileNotFoundError:
        return fallback


class LLMInvocationError(Exception):
    """Raised when the LLM backend fails."""


logger = logging.getLogger(__name__)


def generate_reply(
    system_prompt: str,
    message: str,
    response_schema: dict | None = None,
) -> str:
    messages = [
        SystemMessage(content=system_prompt),
        HumanMessage(content=message),
    ]
    kwargs = {}
    if response_schema:
        kwargs["response_format"] = {
            "type": "json_schema",
            "json_schema": response_schema,
        }
    try:
        response = llm.invoke(messages, **kwargs)
        return response.content
    except Exception as exc:
        logger.exception("LLM invocation failed")
        raise LLMInvocationError("LLM invocation failed") from exc
