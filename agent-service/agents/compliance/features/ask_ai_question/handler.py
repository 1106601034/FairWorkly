from pathlib import Path

from llm import generate_reply
from agents.compliance.prompt_builder import COMPLIANCE_PROMPT

from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)

def run(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    reply = generate_reply(COMPLIANCE_PROMPT, req.question)
    return AskAiQuestionResponse(reply=reply)
